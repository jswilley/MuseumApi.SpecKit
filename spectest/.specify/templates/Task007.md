## 🎟️ Speckit Task: FR-007 Buy Museum Tickets

This speckit task details the requirements for an API endpoint that handles the purchase of museum tickets, supporting both general admission and special events.

***

### 📝 Overview

| Attribute | Value |
| :--- | :--- |
| **Feature ID** | FR-007 |
| **Feature Name** | Buy Museum Tickets |
| **Type** | API Endpoint (POST) |
| **Priority** | High |
| **Tags** | `Tickets` |
| **Related Path** | `/buyMuseumTickets` |

***

### 🎯 Goal (User Story)

As a **Museum Visitor**,
I want to **purchase tickets** for general admission or a specific special event,
So that I can **confirm and secure my visit**.

***

### 🛠️ Technical Specification

#### **Endpoint Definition**

* **Method:** `POST`
* **Path:** `/buyMuseumTickets`
* **`operationId`:** `buyMuseumTickets`
* **Content Type:** `application/json` (in request body)

#### **Request Body Model: `TicketPurchaseRequest`**

| Field | Type | Required | Description | Example |
| :--- | :--- | :--- | :--- | :--- |
| `CustomerEmail` | **string** | **Yes** | Email for confirmation and ticket delivery. | `"user@example.com"` |
| `PaymentToken` | **string** | **Yes** | Secure token from a payment processor (e.g., Stripe, PayPal). | `"tok_abcdef12345"` |
| `TicketType` | **string** | **Yes** | Specifies the type: `"General"` or `"SpecialEvent"`. | `"SpecialEvent"` |
| `VisitDate` | **date** (ISO 8601) | **Yes** | The date the tickets are for. | `"2026-03-15"` |
| `EventId` | **integer/string** | Conditional | **Required** if `TicketType` is `"SpecialEvent"`. | `405` |
| `Quantity` | **integer** | **Yes** | The number of tickets to purchase. | `2` |

#### **Response Body Model: `MuseumTicketConfirmation`**

This object is returned upon successful purchase.

| Field | Type | Description | Example |
| :--- | :--- | :--- | :--- |
| `ConfirmationId` | **string** | A unique transaction identifier. | `"TIX-20260315-A1B2C3"` |
| `TotalAmountPaid` | **number** (decimal) | The final amount charged. | `50.00` |
| `TicketsIssued` | **integer** | The number of tickets successfully issued. | `2` |
| `IssueDate` | **datetime** | The time of successful transaction. | `"2026-03-01T10:30:00Z"` |

***

### ✅ Acceptance Criteria (AC) and Test Cases

#### **AC 1: Successful Ticket Purchase**

* **Criterion:** A successful POST for a valid general or special event ticket purchase must return an **HTTP 201 Created** response with a single `MuseumTicketConfirmation` object.
* **Test Case 1.1 (General Admission Success):**
    * **Input Body:** Valid purchase request for `TicketType: "General"`, `Quantity: 3`.
    * **Expected Output:** **HTTP 201 Created** with a confirmation object including `TicketsIssued: 3`.
* **Test Case 1.2 (Special Event Success):**
    * **Pre-condition:** Event ID `405` exists and has capacity.
    * **Input Body:** Valid purchase request for `TicketType: "SpecialEvent"`, `EventId: 405`, `Quantity: 1`.
    * **Expected Output:** **HTTP 201 Created** with a confirmation object.

#### **AC 2: Authentication Failure**

* **Criterion:** If the request lacks a valid authentication token, the API must return an **HTTP 401 Unauthorized** response.
* **Test Case 2.1 (Missing Token):**
    * **Input:** A valid JSON object matching the `SpecialEvent` schema with all required fields. without an `Authorization` header.
    * **Expected Output:** **HTTP 401 Unauthorized**.

#### **AC 3: Authorization Failure (Incorrect Role)**

* **Criterion:** If the authenticated user does not possess the required **`administrator`** role, the API must return an **HTTP 403 Forbidden** response.
* **Test Case 3.1 (Non-Admin Role):**
    * **Pre-condition:** Request includes a valid token for a user with the **`standard`** role.
    * **Input:** A valid JSON object matching the `SpecialEvent` schema with all required fields.
    * **Expected Output:** **HTTP 403 Forbidden** with an `ErrorModel` detailing the permission denial.


#### **AC 4: Failure on Bad Request**

* **Criterion:** If the purchase fails due to invalid or missing data (e.g., invalid email, missing required fields), the API must return an **HTTP 400 Bad Request** response.
* **Test Case 4.1 (Missing Required Field):**
    * **Input Body:** Request missing the required `PaymentToken`.
    * **Expected Output:** **HTTP 400 Bad Request** indicating the missing field.
* **Test Case 4.2 (Conditional Field Missing):**
    * **Input Body:** Request with `TicketType: "SpecialEvent"` but missing the required `EventId`.
    * **Expected Output:** **HTTP 400 Bad Request** indicating `EventId` is required for the specified ticket type.

#### **AC 5: External Failure/Unavailable Resource**

* **Criterion:** If the purchase fails due to external factors (e.g., payment rejection, event sold out, event not found), the API must return an appropriate error.
* **Test Case 5.1 (Event Not Found):**
    * **Input Body:** Request for `TicketType: "SpecialEvent"` with a non-existent `EventId: 99999`.
    * **Expected Output:** **HTTP 404 Not Found** with a structured error model indicating the event ID was not found.
* **Test Case 5.2 (Payment or Capacity Failure):**
    * **Input Body:** Request where the `PaymentToken` is rejected by the processor, OR the `Quantity` exceeds remaining capacity.
    * **Expected Output:** **HTTP 400 Bad Request** with details explaining the specific failure (e.g., "Payment failed," or "Exceeds remaining ticket capacity").

***

### 🔄 Response Codes and Models

| Code | Status | Model | Description |
| :--- | :--- | :--- | :--- |
| **201** | Created | `MuseumTicketConfirmation` | The ticket purchase was successfully processed and confirmed. |
| **400** | Bad Request | `ErrorModel` | Invalid input data, payment rejection, or capacity exceeded. |
| **404** | Not Found | `ErrorModel` | The specified `EventId` or resource does not exist. |


### Implementation for FR-007
- [FR-007] Add a method to [Service] in museumeapi/services/TicketService.cs. namespace: MuseumApi.Core. Method named BuyTicket, parameter: Ticket, return Ticket class.  The method will handle pulls or pushes the the database.   Add the the existing an interface called ITicketService.  it can be passed to the TicketEndpoint method.   Make sure the register it as scoped in program.cs  
- [FR-007] Add a method to [endpoint/feature] in museumapi/endpoint/TicketEndpoint.cs  It will implement the description above the the inputs/outs.  It will pass the Ticket class to the TicketService's method called BuyTicket.   endpoints should be versioned V1 in the url
- [FR-007]  Add validation and error handling to the Endpoint layer. 
- [FR-007] Add logging for FR-007 operations


### **Logging"**
-- Add logging on successful (informational level) with input parameters.
-- Add logging on error with inputput parameters and structured exceptions.

### **Tests for FR-002**
- [ ] [FR-002] Integration test for in MuseumApi.Test/integration/test_Tickets.cs using xunit.  Try and obtain full coverage.
