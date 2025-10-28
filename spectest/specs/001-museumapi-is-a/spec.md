# Feature Specification: Museum API

**Feature Branch**: `001-museumapi-is-a`  
**Created**: October 21, 2025  
**Status**: Draft  
**Input**: User description: "MuseumApi is a project that will allow users to query museum hours and special events as well as buy tickets to the museum or special event. The api will also allow administrators to insert, update, delete special events."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Query Museum Hours (Priority: P1)

Visitors need to know when the museum is open so they can plan their visit. The system provides current and upcoming museum operating hours.

**Why this priority**: This is the most fundamental information visitors need. Without knowing hours, no visit planning is possible.

**Independent Test**: Can be fully tested by requesting museum hours for specific dates and verifying accurate operating times are returned, delivering immediate value to visitors planning their trips.

**Acceptance Scenarios**:

1. **Given** a visitor is planning a museum visit, **When** they request museum hours for today, **Then** the system returns today's opening and closing times
2. **Given** a visitor is planning a future visit, **When** they request museum hours for a specific date, **Then** the system returns the operating hours for that date
3. **Given** the museum is closed on a particular day, **When** a visitor requests hours for that date, **Then** the system indicates the museum is closed

---

### User Story 2 - Browse Special Events (Priority: P1)

Visitors want to discover special exhibitions, workshops, and events happening at the museum to enhance their visit experience.

**Why this priority**: Special events are a major draw for visitors and directly impact ticket sales and attendance. This is core to the museum's value proposition.

**Independent Test**: Can be fully tested by retrieving the list of special events with details (name, description, dates, price) and verifying completeness and accuracy.

**Acceptance Scenarios**:

1. **Given** there are upcoming special events, **When** a visitor requests the list of special events, **Then** the system returns all current and future events with names, descriptions, dates, and pricing
2. **Given** a visitor is interested in a specific event, **When** they request details for that event by identifier, **Then** the system returns complete event information including all scheduled dates
3. **Given** a visitor wants to plan ahead, **When** they filter events by date range, **Then** the system returns only events occurring within that timeframe

---

### User Story 3 - Purchase Museum Tickets (Priority: P2)

Visitors need to purchase general admission tickets to enter the museum.

**Why this priority**: Revenue generation is critical, but visitors can potentially purchase tickets at the door. Online purchase is a convenience feature that improves user experience.

**Independent Test**: Can be fully tested by submitting a ticket purchase request with visitor details and payment information, then verifying successful transaction processing and ticket confirmation.

**Acceptance Scenarios**:

1. **Given** a visitor wants to visit the museum, **When** they submit a purchase request for general admission tickets with quantity and visit date, **Then** the system processes the transaction and confirms the ticket purchase
2. **Given** a visitor selects a visit date, **When** they complete the purchase, **Then** the system validates the museum is open on that date before processing
3. **Given** a purchase is successful, **When** the transaction completes, **Then** the visitor receives confirmation with ticket details and visit information

---

### User Story 4 - Purchase Special Event Tickets (Priority: P2)

Visitors want to purchase tickets to attend special events, exhibitions, or workshops at the museum.

**Why this priority**: Special event tickets are a key revenue stream, but requires the special events catalog to exist first (dependency on P1).

**Independent Test**: Can be fully tested by submitting a special event ticket purchase for a specific event and date, verifying pricing calculation and successful transaction.

**Acceptance Scenarios**:

1. **Given** a visitor wants to attend a special event, **When** they submit a purchase request specifying the event and date, **Then** the system calculates the total cost and processes the purchase
2. **Given** a special event has limited capacity, **When** a visitor attempts to purchase tickets, **Then** the system validates availability before confirming the purchase
3. **Given** a visitor purchases tickets for multiple events, **When** they complete the transaction, **Then** the system confirms all event tickets separately with dates and details

---

### User Story 5 - Manage Special Events (Admin) (Priority: P3)

Museum administrators need to create, update, and remove special events to keep the event calendar current and accurate.

**Why this priority**: While important for operations, event management can initially be handled manually or through direct database access. This can be built after the public-facing features are stable.

**Independent Test**: Can be fully tested by an administrator creating a new event with all details, updating event information, and removing obsolete events, verifying changes are reflected in the public event listings.

**Acceptance Scenarios**:

1. **Given** an administrator has event details, **When** they submit a request to create a new special event with name, description, dates, and price, **Then** the system stores the event and makes it available to visitors
2. **Given** an administrator needs to update event information, **When** they submit changes to an existing event, **Then** the system updates the event details while preserving the event identifier
3. **Given** an event is cancelled or completed, **When** an administrator requests to delete the event, **Then** the system removes the event from public listings
4. **Given** a special event occurs on multiple dates, **When** an administrator creates or updates the event, **Then** the system allows specification of multiple occurrence dates

---

## Implementation Notes: Admin Special Events

This section documents the concrete admin API implemented for Story 5 and how it is intended to be used and tested.

API surface (versioned v1):

- POST /v1/admin/specialevents
	- Request: `SpecialEventCreateRequest` { EventName, EventDescription, Price, InitialDates?: DateOnly[] }
	- Response: 201 Created with `SpecialEventAdminResponse` (EventId, name, description, price, dates[])

- PUT /v1/admin/specialevents/{id}
	- Request: `SpecialEventUpdateRequest` { EventName?: string, EventDescription?: string, Price?: decimal, ReplaceDates?: DateOnly[] }
	- Response: 200 OK with updated `SpecialEventAdminResponse` or 404 if not found

- DELETE /v1/admin/specialevents/{id}
	- Response: 204 No Content on success, 404 if not found

- POST /v1/admin/specialevents/{id}/dates
	- Request: `SpecialEventDateAddRequest` { Date: DateOnly }
	- Response: 200 OK with updated `SpecialEventAdminResponse` on success, 400 for duplicate date, 404 if event not found

- DELETE /v1/admin/specialevents/{id}/dates/{date}
	- Response: 200 OK with updated `SpecialEventAdminResponse` on success, 400 if date missing, 404 if event not found

Validation & behaviors implemented:

- Price must be non-negative. Negative prices return 400.
- Duplicate event dates for a given event are prevented and return 400 with an explanatory message.
- Operations on non-existent events return 404.
- Deleting an event removes associated dates via cascade delete (DB FK configured).

Testing notes:

- Integration tests were added in `MuseumApi.Tests/IntegrationTests/AdminSpecialEventsIntegrationTests.cs` verifying:
	- Create with initial dates (201)
	- Adding a duplicate date returns 400
	- Updating price and returning updated DTO (200)
	- Deleting event returns 204 and subsequent GET returns 404
	- Adding and removing a date succeeds (200)
	- Non-existent event operations return 404

Security note:

- The implemented admin endpoints are not yet gated by authentication/authorization in this exercise. FR-015 specifies admin restriction; in a real deployment these endpoints must be protected by proper authentication and role-based authorization (e.g., JWT, Azure AD, or similar).

- For local development and integration tests in this repository, the admin endpoints are protected by a development-only test authentication handler.
	- To authenticate admin requests during local testing, include the header `X-Test-Admin: true` on requests. The test handler will create an authenticated principal with the claim `role=admin` and allow access to the admin endpoints.
	- This header-based mechanism is intentionally simple and must be replaced with proper authentication for production deployments.

Try it: curl examples (local development)

Replace HOST:PORT with your local address (e.g., localhost:5000). All admin requests require the `X-Test-Admin: true` header for local testing.

- Create a special event:

```bash
curl -i -X POST "http://HOST:PORT/v1/admin/specialevents" \
	-H "Content-Type: application/json" \
	-H "X-Test-Admin: true" \
	-d '{
		"EventName": "Autumn Concert",
		"EventDescription": "Evening performance in the wing",
		"Price": 25.00,
		"InitialDates": ["2025-11-20","2025-11-21"]
	}'
```

- Update an event (partial):

```bash
curl -i -X PUT "http://HOST:PORT/v1/admin/specialevents/<EVENT_ID>" \
	-H "Content-Type: application/json" \
	-H "X-Test-Admin: true" \
	-d '{ "Price": 30.00 }'
```

- Add a single date to an event:

```bash
curl -i -X POST "http://HOST:PORT/v1/admin/specialevents/<EVENT_ID>/dates" \
	-H "Content-Type: application/json" \
	-H "X-Test-Admin: true" \
	-d '{ "Date": "2025-11-22" }'
```

- Remove a date from an event:

```bash
curl -i -X DELETE "http://HOST:PORT/v1/admin/specialevents/<EVENT_ID>/dates/2025-11-22" \
	-H "X-Test-Admin: true"
```

- Delete an event:

```bash
curl -i -X DELETE "http://HOST:PORT/v1/admin/specialevents/<EVENT_ID>" \
	-H "X-Test-Admin: true"
```

Notes:

- Use the `-i` flag to see HTTP response headers (status codes).
- Dates are formatted as `YYYY-MM-DD` in requests and route segments.
- For production, replace the `X-Test-Admin` header with proper authentication (bearer tokens/JWT, etc.).


---

### Edge Cases

- What happens when a visitor requests museum hours for a date far in the future for which hours haven't been scheduled yet?
- How does the system handle ticket purchases for dates when the museum is closed?
- What happens when an administrator attempts to delete a special event that has already sold tickets?
- How does the system handle concurrent ticket purchases when limited capacity is reached?
- What happens when a special event price is updated after some tickets have already been sold?
- How does the system handle ticket purchases for special events on dates when the event isn't scheduled?

## Requirements *(mandatory)*

### Functional Requirements

#### Museum Hours

- **FR-001**: System MUST provide current and future museum operating hours including opening time, closing time, and date
- **FR-002**: If a operating hours record is not found for a particular day, the system will assume the museum is closed on that day.
- **FR-003**: System MUST allow querying dates specific dates or date ranges

#### Special Events

- **FR-004**: System MUST maintain a catalog of special events including event name, description, and pricing information
- **FR-005**: System MUST associate each special event with one or more specific dates when the event occurs
- **FR-006**: System MUST allow retrieval of all available special events
- **FR-007**: System MUST allow retrieval of a specific special event by its unique identifier
- **FR-008**: System MUST support filtering special events by date or date range

#### Ticket Purchasing

- **FR-009**: System MUST process purchase requests for general museum admission tickets
- **FR-010**: System MUST process purchase requests for special event tickets
- **FR-011**: System MUST validate that requested visit dates correspond to days when the museum is open
- **FR-012**: System MUST validate that special event ticket purchases are for dates when the event is scheduled
- **FR-013**: System MUST calculate total purchase cost based on ticket quantity and applicable pricing
- **FR-014**: System MUST provide purchase confirmation with ticket details upon successful transaction

#### Administrative Functions

- **FR-015**: System MUST restrict administrative functions to authorized administrators only
- **FR-016**: Administrators MUST be able to create new special events with all required details (name, description, dates, price)
- **FR-017**: Administrators MUST be able to update existing special event information
- **FR-018**: Administrators MUST be able to delete special events from the system
- **FR-019**: System MUST maintain referential integrity when administrators modify or delete events that may have associated data

#### Data Validation

- **FR-020**: System MUST validate that special event prices are positive values
- **FR-021**: System MUST validate that event dates are valid calendar dates
- **FR-022**: System MUST prevent duplicate museum hours entries for the same date
- **FR-023**: System MUST prevent duplicate special event date assignments for the same event and date combination

### Key Entities

- **Museum Hours**: Represents daily operating schedule including date, opening time, and closing time. Each date has at most one hours entry. Closed days may be represented by absence of entry or explicit closed status.

- **Special Event**: Represents exhibitions, workshops, or special programs with unique identifier, name, descriptive information, and pricing. Each event exists independently and can be scheduled for multiple dates.

- **Event Date**: Associates a special event with specific dates when it occurs. Links events to calendar dates, enabling the same event to run on multiple days.

- **Ticket Purchase**: Represents a transaction for museum admission or special event attendance. Includes purchase details, ticket quantity, visit date, and associated event (if applicable). Links visitors to their intended visit dates and events.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Visitors can retrieve museum hours for any requested date in under 2 seconds
- **SC-002**: Visitors can browse the complete special events catalog and view event details in under 3 seconds
- **SC-003**: Visitors can complete a ticket purchase transaction from selection to confirmation in under 60 seconds
- **SC-004**: System accurately processes ticket purchases 99.9% of the time without errors
- **SC-005**: Administrators can create, update, or delete special events with changes reflected immediately in public listings
- **SC-006**: System prevents invalid data entry (negative prices, invalid dates, duplicate entries) 100% of the time
- **SC-007**: 95% of visitors successfully find the information they need (hours or events) on their first query
- **SC-008**: System supports at least 100 concurrent ticket purchase transactions without performance degradation

## Assumptions

- Museum hours are scheduled in advance and loaded into the system by administrators or automated processes
- Payment processing integration details will be defined during technical planning
- User authentication for administrators will be handled by existing organizational systems
- Ticket inventory and capacity management will be defined based on business requirements during planning
- The system will be accessed programmatically by other applications rather than providing a user interface
- General admission pricing is consistent and doesn't require per-date variation like special events

## Dependencies

- Access to museum scheduling system or process for obtaining official operating hours
- Payment processing service or gateway for handling ticket transactions
- User authentication and authorization system for administrator access
- Organizational policies regarding ticket sales, refunds, and event management

## Out of Scope

- Physical ticket printing or QR code generation
- Customer account management and login for ticket purchasers
- Refund processing and ticket cancellations
- Email notifications or confirmations
- Membership programs or discounted pricing tiers
- Gift shop or merchandise sales
- Event capacity management and sold-out status
- Multi-language support
- Mobile application development
- Marketing and promotional features
