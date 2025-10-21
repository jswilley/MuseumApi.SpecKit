## 🚫 Speckit Task: FR-003 Special Event Price Validation

This speckit task details the validation requirement to ensure all special events have a positive price. Since this is a validation task, it primarily affects existing or future API endpoints that handle event creation or modification (like `POST /SpecialEvent` from FR-002).

***

### 📝 Overview

| Attribute | Value |
| :--- | :--- |
| **Feature ID** | FR-003 |
| **Feature Name** | Special Event Price Validation |
| **Type** | Data Validation/Constraint |
| **Priority** | High |
| **Affected Model** | `SpecialEvent` |
| **Affected Field** | `EventPrice` |
| **Constraint** | Must be $> 0$ |

***

### 🎯 Goal (User Story)

As a **System Integrity Manager**,
I want the system to **enforce that all special event prices are positive**,
So that **event data remains accurate** and prevents erroneous free/negative charges.

***

### 🛠️ Technical Specification

#### **Validation Constraint**

* **Model:** `SpecialEvent`
* **Field:** `EventPrice` (type: number/decimal)
* **Rule:** The value of `EventPrice` **must be greater than zero** ($> 0$).
* **Scope:** This validation must be applied at the service layer for any operation that accepts or modifies the `SpecialEvent` model (e.g., `POST /SpecialEvent`, `PUT /SpecialEvent/{id}`).

***

### ✅ Acceptance Criteria (AC) and Test Cases

#### **AC 1: Price Rejection on Non-Positive Value**

* **Criterion:** If a request is made to add or update a `SpecialEvent` with a non-positive `EventPrice` (i.e., $\le 0$), the API must reject the request with an **HTTP 400 Bad Request** response.
* **Test Case 1.1 (Zero Price Rejection):**
    * **Action:** Attempt to create an event via `POST /SpecialEvent` with `EventPrice: 0.00`.
    * **Expected Output:** **HTTP 400 Bad Request** with a structured error model indicating that `EventPrice` must be positive.
* **Test Case 1.2 (Negative Price Rejection):**
    * **Action:** Attempt to create an event via `POST /SpecialEvent` with `EventPrice: -5.00`.
    * **Expected Output:** **HTTP 400 Bad Request** with a structured error model indicating that `EventPrice` must be positive.
* **Test Case 1.3 (Successful Positive Price):**
    * **Action:** Attempt to create an event via `POST /SpecialEvent` with `EventPrice: 1.00`.
    * **Expected Output:** **HTTP 201 Created**. (Standard success based on FR-002).

***

### 🔄 Response Codes and Models

| Code | Status | Model | Description |
| :--- | :--- | :--- | :--- |
| **201** | Created | `SpecialEvent` (Output) | Success (when price is positive, used in conjunction with FR-002). |
| **400** | Bad Request | `ErrorModel` | The validation failed because `EventPrice` was not a positive value. |

* *Note: **ErrorModel** should contain fields like `code`, `message`, and `details` to specifically point out the `EventPrice` validation failure.*


### **Logging"**
-- Add logging on successful (informational level) with input parameters.
-- Add logging on error with inputput parameters and structured exceptions.