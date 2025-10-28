## 🔎 Speckit Task: FR-004 Query Special Event

This speckit task specifies the details for retrieving a single special event using its unique identifier.

***

### 📝 Overview

| Attribute | Value |
| :--- | :--- |
| **Feature ID** | FR-004 |
| **Feature Name** | Query Special Event by ID |
| **Type** | API Endpoint (GET) |
| **Priority** | High |
| **Related Path** | `/SpecialEvent/{EventId}` |

***

### 🎯 Goal (User Story)

As a **Museum Visitor** or **Event Administrator**,
I want to **retrieve the detailed information for a specific special event** using its ID,
So that I can **view its schedule, location, and price**.

***

### 🛠️ Technical Specification

#### **Endpoint Definition**

* **Method:** `GET`
* **Path:** `/SpecialEvent/{EventId}`
* **`operationId`:** `getSpecialEvent`

#### **Parameters (Path)**

| Name | Type | Required | Location | Description |
| :--- | :--- | :--- | :--- | :--- |
| `EventId` | **integer** (or **string**) | **Yes** | Path | The unique system identifier for the special event. |

*Note: The specification lists `EventId` type as **date**, which is highly unusual for an identifier. Assuming it should be an **integer** or **string** based on common API practices for IDs.*

#### **Response Body Model: `SpecialEvent`**

The response body will use the standard `SpecialEvent` model (as defined in FR-002, likely including fields like `EventID`, `EventName`, `EventLocation`, `EventDescription`, `EventDates`, and `EventPrice`).

***

### ✅ Acceptance Criteria (AC) and Test Cases

#### **AC 1: Successful Event Retrieval**

* **Criterion:** A successful request to get a special event must return an **HTTP 200 OK** response containing a JSON of a single `SpecialEvent` object.
* **Test Case 1.1 (Valid ID Retrieval):**
    * **Pre-condition:** Event with ID `101` exists in the system.
    * **Input:** `GET /SpecialEvent/101`
    * **Expected Output:** **HTTP 200 OK** with a JSON object representing the special event details, where `EventID` is `101`.

#### **AC 3: Event Not Found**

* **Criterion:** If the requested `EventId` does not correspond to an existing special event, the API must return an **HTTP 404 Not Found** response.
* **Test Case 3.1 (Non-existent ID):**
    * **Input:** `GET /SpecialEvent/99999` (assuming this ID is not in the system)
    * **Expected Output:** **HTTP 404 Not Found** with a structured error model indicating the resource was not found.

#### **AC 4: Invalid Parameter Format**

* **Criterion:** If the `EventId` format in the path is invalid (e.g., non-numeric when an integer is expected), the API must return an **HTTP 400 Bad Request** response.
* **Test Case 4.1 (Invalid Format):**
    * **Input:** `GET /SpecialEvent/INVALID_ID`
    * **Expected Output:** **HTTP 400 Bad Request** with a structured error model indicating a path parameter format error.

***

### 🔄 Response Codes and Models

| Code | Status | Model | Description |
| :--- | :--- | :--- | :--- |
| **200** | OK | `SpecialEvent` | Successfully retrieved the requested event details. |
| **400** | Bad Request | `ErrorModel` | The path parameter (`EventId`) was provided in an invalid format. |
| **404** | Not Found | `ErrorModel` | No special event could be found with the provided `EventId`. |



### Implementation for FR-004
- [FR-004] Adds a method to [Service] in museumeapi/services/SpecialEventService.cs.  namespace: MuseumApi.Core. it add a method named GetSpecialEvent.  it will take the following parameters: parameters EventId return SpecialEvent.  The method will handle pulls or pushes the the database.   Add the the existing an interface called ISpecialEventService.  it can be passed to the SpecialEventEndpoint method. 
- [FR-004] Adds a method to [endpoint/feature] in museumapi/endpoint/SpecialEventEndpoint.cs  It will implement the description above the the inputs/outs.  It will pass the EventId to the SpecialEventService's method called GetSpecialEvent.   endpoints should be versioned V1 in the url
- [FR-004] Add validation and error handling to the Endpoint layer.
- [FR-004] Add logging for FR-004 operations


### **Logging"**
-- Add logging on successful (informational level) with input parameters.
-- Add logging on error with inputput parameters and structured exceptions.

### **Tests for FR-004**
- [ ] [FR-004] Integration test for in MuseumApi.Test/integration/test_SpecialEvent.cs using xunit.  Try and obtain full coverage.
