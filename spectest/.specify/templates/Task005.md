## ✍️ Speckit Task: FR-005 Update Special Event

This speckit task details the requirements for updating an existing special event via the API using a **PUT** operation.

***

### 📝 Overview

| Attribute | Value |
| :--- | :--- |
| **Feature ID** | FR-005 |
| **Feature Name** | Update Special Event |
| **Type** | API Endpoint (PUT) |
| **Priority** | High |
| **Tags** | `Events` |
| **Related Path** | `/SpecialEvent/{EventId}` (Standard convention for PUT) |

***

### 🎯 Goal (User Story)

As a **Museum Administrator**,
I want to **modify the details of an existing special event** in the system,
So that I can **correct errors or update logistics** like the price or dates.

***

### 🛠️ Technical Specification

#### **Endpoint Definition**

* **Method:** `PUT`
* **Path:** `/SpecialEvent/{EventId}`
* **`operationId`:** `updateSpecialEvent`
* **Content Type:** `application/json` (in request body)

*Note: While the requirement specified the path as `/SpecialEvent`, standard REST practices for a full update of a resource use the ID in the path, e.g., `/SpecialEvent/{EventId}`. The `EventId` must also be included in the body for **PUT** operations when the resource ID isn't exclusively in the path.*

#### **Parameters (Path)**

| Name | Type | Required | Location | Description |
| :--- | :--- | :--- | :--- | :--- |
| `EventId` | **integer** (or **string**) | **Yes** | Path | The unique system identifier of the event to be updated. |

#### **Request Body Model: `SpecialEvent` (Input for PUT)**

The request body represents the **full, new state** of the `SpecialEvent` object.

| Field | Type | Required | Constraint/Notes |
| :--- | :--- | :--- | :--- |
| `EventId` | **integer** (or **string**) | **Yes** | Must match the ID in the path. |
| `EventName` | **string** | **Yes** | Full new name. |
| `EventLocation` | **string** | **Yes** | Full new location. |
| `EventDescription` | **string** | **Yes** | Full new description. |
| `EventDates` | **array of date** | **Yes** | Full list of new dates (ISO 8601). |
| `EventPrice` | **number** (decimal) | **Yes** | Full new price (must be positive, per FR-003). |

#### **Response Body Model**

The successful response should return the **fully updated** object.

| Field | Type | Description |
| :--- | :--- | :--- |
| *[All Input Fields]* | *[Same as above]* | The complete, updated event record. |

***

### ✅ Acceptance Criteria (AC) and Test Cases

#### **AC 1: Successful Event Update**

* **Criterion:** A successful PUT request with a valid body and existing `EventId` must return an **HTTP 200 OK** response with the single, updated `SpecialEvent` object.
* **Test Case 1.1 (Full Update Success):**
    * **Pre-condition:** Event with ID `101` exists.
    * **Input:** `PUT /SpecialEvent/101` with a valid JSON body containing updated details (e.g., changed `EventPrice` from $10 to $15).
    * **Expected Output:** **HTTP 200 OK** with the JSON object for event `101` reflecting the new price of $15.

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


#### **AC 4: Failure on Bad Request (Validation)**

* **Criterion:** If the update fails due to invalid data (e.g., missing fields, incorrect type, or validation violations like non-positive price), the API must return an **HTTP 400 Bad Request** response.
* **Test Case 4.1 (Missing Required Field):**
    * **Input Body:** A JSON object for `PUT /SpecialEvent/101` where a required field (e.g., `EventName`) is missing.
    * **Expected Output:** **HTTP 400 Bad Request** with a structured error model indicating the missing field.
* **Test Case 4.2 (Price Validation Failure - FR-003):**
    * **Input Body:** A JSON object for `PUT /SpecialEvent/101` where `EventPrice: 0.00`.
    * **Expected Output:** **HTTP 400 Bad Request** with a structured error model indicating `EventPrice` must be positive.

#### **AC 5: Event Not Found**

* **Criterion:** If the requested `EventId` in the path does not correspond to an existing event, the API must return an **HTTP 404 Not Found** response.
* **Test Case 5.1 (Non-existent ID):**
    * **Input:** `PUT /SpecialEvent/99999` (assuming this ID is not in the system) with a valid JSON body.
    * **Expected Output:** **HTTP 404 Not Found** with a structured error model indicating the resource for ID `99999` was not found.

***

### 🔄 Response Codes and Models

| Code | Status | Model | Description |
| :--- | :--- | :--- | :--- |
| **200** | OK | `SpecialEvent` | The special event was successfully updated. |
| **400** | Bad Request | `ErrorModel` | The request body contained invalid or incomplete data (validation failure). |
| **404** | Not Found | `ErrorModel` | No special event could be found with the provided `EventId` to update. |

### Implementation for FR-005
- [FR-005] Add a method to [Service] in museumeapi/services/SpecialEventService.cs. namespace: MuseumApi.Core. Method named UpdateSpecialEvent, parameter: SpecialEvent, return SpecialEvent class.  The method will handle pulls or pushes the the database.  It will use entityframework's UpsertFunctionality to update the database tables.   Add the the existing an interface called ISpecialEventService.  it can be passed to the SpecialEventEndpoint method. 
- [FR-005] Add a method to [endpoint/feature] in museumapi/endpoint/SpecialEventEndpoint.cs  It will implement the description above the the inputs/outs.  It will pass the SpecialEvent class to the SpecialEventService's method called UpdateSpecialEvent.   endpoints should be versioned V1 in the url
- [FR-005]  Add validation and error handling to the Endpoint layer.  Price must be a postive decimal value.
- [FR-005] Add logging for FR-005 operations


### **Logging"**
-- Add logging on successful (informational level) with input parameters.
-- Add logging on error with inputput parameters and structured exceptions.

### **Authorization**
-- you must have the role of Administrator to update a SpecialEvent

### **Tests for FR-005**
- [ ] [FR-005] Integration test for in MuseumApi.Test/integration/test_SpecialEvent.cs using xunit.  Try and obtain full coverage.
