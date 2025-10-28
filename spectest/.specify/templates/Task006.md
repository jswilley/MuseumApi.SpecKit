## ❌ Speckit Task: FR-006 Delete Special Event

This speckit task details the requirements for permanently removing a special event from the system using a **DELETE** operation.

***

### 📝 Overview

| Attribute | Value |
| :--- | :--- |
| **Feature ID** | FR-006 |
| **Feature Name** | Delete Special Event |
| **Type** | API Endpoint (DELETE) |
| **Priority** | High |
| **Tags** | `Events` |
| **Related Path** | `/SpecialEvent/{EventId}` |

***

### 🎯 Goal (User Story)

As a **Museum Administrator**,
I want to **permanently delete a cancelled or invalid special event** from the system,
So that **visitors are no longer able to view or book it**.

***

### 🛠️ Technical Specification

#### **Endpoint Definition**

* **Method:** `DELETE`
* **Path:** `/SpecialEvent/{EventId}`
* **`operationId`:** `deleteSpecialEvent`

#### **Parameters (Path)**

| Name | Type | Required | Location | Description |
| :--- | :--- | :--- | :--- | :--- |
| `EventId` | **integer** (or **string**) | **Yes** | Path | The unique system identifier for the event to be deleted. |

*Note: The specification lists `EventId` type as **date**, which is highly unusual for an identifier. Assuming it should be an **integer** or **string** based on common API practices for IDs.*

#### **Request/Response Body**

* **Request Body:** **None** (The identifier is in the path).
* **Successful Response Body:** **None** (Standard practice for HTTP **204 No Content**).

***

### ✅ Acceptance Criteria (AC) and Test Cases

#### **AC 1: Successful Event Deletion**

* **Criterion:** A successful DELETE request for an existing special event must return an **HTTP 204 No Content** response.
* **Test Case 1.1 (Valid Deletion):**
    * **Pre-condition:** Event with ID `202` exists in the system.
    * **Input:** `DELETE /SpecialEvent/202`
    * **Expected Output:** **HTTP 204 No Content**. A subsequent GET request for `/SpecialEvent/202` should return a **404 Not Found**.


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

#### **AC 4: Event Not Found**

* **Criterion:** If the requested `EventId` does not correspond to an existing special event, the API must return an **HTTP 404 Not Found** response.
* **Test Case 4.1 (Non-existent ID):**
    * **Input:** `DELETE /SpecialEvent/88888` (assuming this ID is not in the system)
    * **Expected Output:** **HTTP 404 Not Found** with a structured error model indicating the resource was not found.

***

### 🔄 Response Codes and Models

| Code | Status | Model | Description |
| :--- | :--- | :--- | :--- |
| **204** | No Content | *None* | The special event was successfully deleted. |
| **400** | Bad Request | `ErrorModel` | The path parameter (`EventId`) was provided in an invalid format. |
| **401** | Unauthorized | `ErrorModel` | Authentication credentials were missing or invalid. |
| **404** | Not Found | `ErrorModel` | No special event could be found with the provided `EventId`. |



### Implementation for FR-006
- [FR-006] Add a method to [Service] in museumeapi/services/SpecialEventService.cs.  namespace: MuseumApi.Core. Method named DeleteSpecialEvent, parameter: EventId, return 1 if successful.  The method will handle pulls or pushes the the database.  It will use entityframework's UpsertFunctionality to update the database tables.   Add the the existing an interface called ISpecialEventService.  it can be passed to the SpecialEventEndpoint method. 
- [FR-006] Add a method to [endpoint/feature] in museumapi/endpoint/SpecialEventEndpoint.cs  It will implement the description above the the inputs/outs.  It will pass the EventId to the SpeialEventService's method called eleteSpecialEvent.    endpoints should be versioned V1 in the url
- [FR-006]  Add validation and error handling to the Endpoint layer. 
- [FR-006] Add logging for FR-006 operations


### **Logging"**
-- Add logging on successful (informational level) with input parameters.
-- Add logging on error with inputput parameters and structured exceptions.

### **Authorization**
-- you must have the role of Administrator to delete a SpecialEvent

### **Tests for FR-006**
- [ ] [FR-006] Integration test for in MuseumApi.Test/integration/test_SpecialEvent.cs using xunit.  Try and obtain full coverage.
