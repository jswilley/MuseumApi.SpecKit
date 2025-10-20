# Tasks: Museum Catalog API Implementation

**Input**: Design documents from `/specs/001-implementation/`
**Prerequisites**: plan.md (required), spec.md (required)
**Tests**: Required per plan.md with minimum 60% coverage requirement

## Format: `[ID] [P?] [Story] Description`
- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to

## Phase 1: Setup (Project Infrastructure)

**Purpose**: Initialize .NET project and establish core infrastructure

- [ ] T001 Create MuseumApi.csproj as .NET 8 Web API project
- [ ] T002 [P] Add SQLite and Entity Framework Core packages
- [ ] T003 [P] Add Serilog packages for logging
- [ ] T004 Configure basic appsettings.json with connection strings
- [ ] T005 Create initial Program.cs with minimal setup

## Phase 2: Database Setup

**Purpose**: Establish data persistence layer

- [ ] T006 Create /Data directory and initialize SQLite database
- [ ] T007 [P] Create Models/SpecialEvent.cs entity
- [ ] T008 [P] Create Models/MuseumDailyHours.cs entity
- [ ] T009 [P] Create Models/Ticket.cs entity
- [ ] T010 Create Context/MuseumContext.cs for Entity Framework
- [ ] T011 Create initial EF migration
- [ ] T012 Implement database seeding with sample data

## Phase 3: Core Services (Tests First)

**Purpose**: Implement core business logic with TDD approach

### Tests
- [ ] T013 [P] Create test project MuseumApi.Tests
- [ ] T014 [P] Write SpecialEventService tests
- [ ] T015 [P] Write MuseumHoursService tests
- [ ] T016 [P] Write TicketService tests

### Implementation
- [ ] T017 Create Services/SpecialEventService.cs (depends on T014)
- [ ] T018 Create Services/MuseumHoursService.cs (depends on T015)
- [ ] T019 Create Services/TicketService.cs (depends on T016)

## Phase 4: API Endpoints (Tests First)

**Purpose**: Implement API endpoints following contract

### Tests
- [ ] T020 [P] Write endpoint tests for special events
- [ ] T021 [P] Write endpoint tests for museum hours
- [ ] T022 [P] Write endpoint tests for tickets

### Implementation
- [ ] T023 Create Endpoints/SpecialEventsEndpoints.cs (depends on T020)
- [ ] T024 Create Endpoints/MuseumHoursEndpoints.cs (depends on T021)
- [ ] T025 Create Endpoints/TicketEndpoints.cs (depends on T022)
- [ ] T026 Register endpoints in Program.cs

## Phase 5: Security & Validation

**Purpose**: Implement security and validation requirements

- [ ] T027 [P] Add authentication middleware for admin endpoints
- [ ] T028 [P] Implement input validation for SpecialEvent
- [ ] T029 [P] Add global error handling middleware
- [ ] T030 Configure security headers

## Phase 6: Testing & Validation

**Purpose**: Ensure quality and specification compliance

- [ ] T031 Run Specmatic contract tests
- [ ] T032 Verify test coverage meets 60% minimum
- [ ] T033 Run integration tests
- [ ] T034 Validate security requirements
- [ ] T035 Document API endpoints

## Notes

- Tasks within phases can be executed in parallel if marked with [P]
- Test tasks must be completed before corresponding implementation tasks
- All security checklist items must pass before deployment

## 🏛️ Speckit Task: FR-001 Query Museum Operating Hours

### 📝 Overview

| Attribute | Value |
| :--- | :--- |
| **Feature ID** | FR-000 |
| **Feature Name** | Project Setup |
| **Priority** | High |


***

### 🎯 Goal (User Story)

The goals is to setup the project in preparation for the more specific tasks to come.

***

### 🛠️ Technical Specification

#### **Visual Studio Projet**
Create a Visual Studio 2022 project for ASP.Net Core minimal API.  The Project name will by MuseumApi.csproj
Download a copy of SQL Lite and store it in a subdirectory called /Data.
Add nuget packages for entityframework core sqlite.
In the program.cs create a dataconnection between the project and the sqlite db stored in the /Data directory.
Add nuget packages for Serilog AspNet.Core and File sinks.


#### **sqlite table creation**



Table name: SpecialEvent
| Name | Type | Required | Default | Description | Validation |
| :--- | :--- | :--- | :--- | :--- |
| `EventID` | **UUID** | **Yes** |  | Unique Identifier for Event. | |
| `EventName` | **string** | **Yes** |  | Name of Event. | Maximum length is 150.|
| `EventDescription` | **string** | **Yes** |  | Description of Event. | Maximum length is 750.|
| 'Price' | **decimal** | **Yes**  | | The ticket price. | must be a positive number |


Table name: SpecialEventDate
| Name | Type | Required | Default | Description | Validation |
| `EventId` | **UUID** | **Yes** |  | Unique Identifier for Event. | Taken from SpecialEvent table row \
| `Date` | **date** (ISO 8601) | **Yes** |  |The specific date for the Event. | The date must be unique for a particular event id |

Table name: MuseumDailyHours
|  Name | Type | Required | Default | Description | Validation |
| `ID` | **integer** | **Yes** | | unique id for row generated by table. | primary key. |
| `Date` | **date** (ISO 8601) | **Yes** | | The specific date for the operating hours. | must be a unique date. |
| `TimeOpen` | **string** (Time) | The opening time of the museum for that date. | `09:00:00` |
| `TimeClosed` | **string** (Time) | The closing time of the museum for that date. | `17:30:00` |

Table name: Ticket
|  Name | Type | Required | Default | Description | Validation |
| `TicketID` | **UUID** | **Yes** | | unique id for row generated by table. | primary key. |
| `TicketDate` | **date** (ISO 8601) | **Yes** | | The specific date for the ticket. | must be a correspond to a MuseumDailyHours Date. |
| `TicketType` | **string** | **Yes** |  | Type of Ticket. | can only be 'General' or 'SpecialEvent'.|
| `EventID` | **UUID** | **No** |  | Unique Identifier from SpecialEvent table. |  If the type is specialevent it must correspond to an eventid in the specialevent table. |

#### **sqlite table data seeding**
Data for MuseumDailyHours
row 1
Date: "2023-09-11"
TimeOpen: "09:00"
TimeClose: "18:00"
row 2
Date: "2023-09-12"
TimeOpen: "09:00"
TimeClose: "18:00"
row 3
Date: "2023-09-13"
TimeOpen: "09:00"
TimeClose: "18:00"
row 4
Date: "2023-09-14"
TimeOpen: "09:00"
Timeclose: "18:00"
row 5
Date: "2023-09-15"
TimeOpen: "10:00"
TimeClose: "16:00"
row 6
Date: "2023-09-18"
TimeOpen: "09:00"
TimeClose: "18:00"
row 7
Date: "2023-09-19"
TimeOpen: "09:00"
TimeClose: "18:00"
row 8
Date: "2023-09-20"
TimeOpen: "09:00"
TimeClose: "18:00"
row 9
Date: "2023-09-21"
TimeOpen: "09:00"
TimeClose: "18:00"
row 10
Date: "2023-09-22"
TimeOpen: "10:00"
TimeClose: "16:00"


Data for SpecialEvent table
row 1
	EventID: f3e0e76e-e4a8-466e-ab9c-ae36c15b8e97
    EventName: Sasquatch Ballet
    EventDescription: They're big, they're hairy, but they're also graceful. Come learn how the biggest feet can have the lightest touch.
	Price: 15.00 
row 2
	EventID: f3e0e76e-e4a8-466e-ab9c-ae36c15b8e98
    EventName: Solar Telescope Demo
    EventDescription: Look at the sun without going blind!
	Price: 10.00 
	
row 3
	EventID: f3e0e76e-e4a8-466e-ab9c-ae36c15b8e77
    EventName: Cook Like a Caveman
    EventDescription: Learn how to cook on open flame.
	Price: 20.00 
row 4
	EventID: dad4bce8-f5cb-4078-a211-995864315e39
    EventName: Mermaid Treasure Identification and Analysis
    EventDescription: Join us as we review and classify a rare collection of 20 thingamabobs, gadgets, gizmos, whoosits, and whatsits — kindly donated by Ariel.
	Price: 10.00 
row 4
	EventID: 6744a0da-4121-49cd-8479-f8cc20526495
    EventName: Time Traveler Tea Party
    EventDescription: Sip tea with important historical figures.
	Price: 30.00 
	

Data for SpecialEventDate table
row 1
	EventID: f3e0e76e-e4a8-466e-ab9c-ae36c15b8e97
	Date: '09/11/2023'
row 2
	EventID: f3e0e76e-e4a8-466e-ab9c-ae36c15b8e97
	Date: '09/12/2023'
row 3
	EventID: f3e0e76e-e4a8-466e-ab9c-ae36c15b8e97
	Date: '09/13/2023'
row 4
	EventID: f3e0e76e-e4a8-466e-ab9c-ae36c15b8e98
	Date: '09/11/2023'
row 5
	EventID: f3e0e76e-e4a8-466e-ab9c-ae36c15b8e98
	Date: '09/14/2023'
row 6
	EventID: f3e0e76e-e4a8-466e-ab9c-ae36c15b8e98
	Date: '09/15/2023'
row 7
	EventID: f3e0e76e-e4a8-466e-ab9c-ae36c15b8e77	
	Date: '09/16/2023'
row 8
	EventID: f3e0e76e-e4a8-466e-ab9c-ae36c15b8e77	
	Date: '09/17/2023'
row 9
	EventID: dad4bce8-f5cb-4078-a211-995864315e39
	Date: '09/17/2023'
row 10
	EventID: dad4bce8-f5cb-4078-a211-995864315e39
	Date: '09/18/2023'
row 11	
	EventID: 6744a0da-4121-49cd-8479-f8cc20526495	
	Date: '09/19/2023'
#### **Classes**
Class should be created in a subdirectory called Models
 Class Name | Description | Attributes |
| :--- | :--- | :--- |
| **SpecialEvent** | The core entity representing a museum SpecialEvent. | `EventId`: **GUID**,`EventName`: string,`EventDescription`: string,`EventDates`: *Items:* **SpecialEventDates**,`Price`: **decimal** |
| **SpecialEventCollection** | List of upcoming events. | `array` | *Items:* **SpecialEvent** |
| **MuseumDailyHours** | The hours of operations for a day. |`Date`: **date**,`TimeOpen`: **time**,`TimeClosed`: **time** |
| **MuseumHours** | The hours of operations for a date range. | *Items:* **MuseumDailyHours** ,`SearchStartDate`: **date**,`SearchEndDate`: **date** |
| **Ticket** | Ticket for museum entry (can be general or special event). |`TicketId`: **GUID**,`TicketDate`: **date**, `TicketType`: **string**,`EventId`: **EventId** |
| **MuseumTicketConfirmation** | Details for a museum ticket after a successful purchase. |`TicketCodeImage`: **string**,`MuseumHours`: **MuseumHours**,`TicketType`: **string**,`EventId`: **GUID**, EventName **string** |
| **BuyMuseumTickets** | Data to purchase a ticket. | `TicketMessage`: **string**, `TicketId`: **GUID**, `TicketConfirmation`: **string** |
| **Error** | Standard error object. |`Type`: **string**`Title`: **string**, 'Details': **string** |


#### **Map Table structure to Classes**
Create a subdirectory called context.  In the directory map the table structure to classes using Entityframework Core.  

#### **Services**
In C# create a 
SpecialEventService Class that will take the endpoints data and reponds with pulls or pushes the the database. Inject the databasee context 
Method 1: name: GetSpecialEvent, parameter: EventID, return SpecialEvent class.
Method 2: name: UpdateSpecialEvent, parameter: SpecialEvent, return SpecialEvent or an Error.  Use EntityFramework's ability to do UpSerts to insert or update the record.
Method 3: name: DeleteSpecialEvent, parameer: EventID, returns 1 or an Error.  

MuseumHourService  Class that will take the endpoints data and reponds with pulls or pushes the the database. Inject the databasee context 
Method 1: name: SearchMuseumHours, parameters StartDate (date), EndDate (date) optional default to today's date, return MuseumDailyHours.

BuyTicketService   Class that will take the endpoints data and reponds with pulls or pushes the the database. Inject the databasee context
Method 1: name: BuyTicket, parameter: Ticket, return MuseumTicketConfirmation.

#### **Endpoints**
Create a subdirectory called endpoints where the static enpoints can be put.

#### **Test Projects** 
Create an xunit a Museum Api test project that references MuseumApi.csproj.
