# WorkflowEngine

A minimal backend service that acts as a configurable workflow engine, implemented as a State-Machine API.

## Objective

This service allows clients to:
1. Define one or more workflows as configurable state machines (states + actions).
2. Start workflow instances from a chosen definition.
3. Execute actions to move an instance between states, with full validation.
4. Inspect/list states, actions, definitions, and running instances.

## Core Concepts

* **State**: Represents a stage in a workflow (e.g., PENDING, SHIPPED). Attributes include `id`, `isInitial`, `isFinal`, `enabled`.
* **Action (transition)**: Defines how to move between states. Attributes include `id`, `enabled`, `fromStates` (a list of states it can originate from), and `toState` (the single state it transitions to).
* **Workflow Definition**: A blueprint for a workflow, comprising a collection of `States` and `Actions`. Must have exactly one initial state.
* **Workflow Instance**: A running instance of a `Workflow Definition`, tracking its `current state` and `basic history`. Starts at the definition's initial state.

## Technical Stack

* **Language**: C#
* **Framework**: .NET 8
* **Web Framework**: ASP.NET Core Minimal APIs
* **Persistence**: In-memory data storage

## Quick-Start Instructions

1.  **Prerequisites:**
    * .NET 9 SDK installed: [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
    * Visual Studio Code (optional, but recommended) with C# Dev Kit extension.

2.  **Clone the repository:**
    ```bash
    git clone <repo-url>
    cd WorkflowEngine
    ```

3.  **Run the application:**
    ```bash
    dotnet run
    ```
    The application will start, usually listening on `https://localhost:5166`. You will see the exact URL in the console output.

4.  **Access API Documentation (Swagger UI):**
    Open your web browser and navigate to `https://localhost:5166/swagger`. Here you can see all available endpoints and interact with them.

## Sample API Usage (via Swagger UI or tools like Postman)

You can use the Swagger UI for easy testing. Here are the main endpoints:

### 1. Create Workflow Definition

* **Endpoint**: `POST /workflows/definitions`
* **Description**: Creates a new workflow definition.
* **Request Body Example**:
    ```json
    {
      "id": "OrderProcessing",
      "name": "Order Processing Workflow",
      "states": [
        { "id": "PENDING", "isInitial": true, "isFinal": false, "enabled": true },
        { "id": "PROCESSING", "isInitial": false, "isFinal": false, "enabled": true },
        { "id": "SHIPPED", "isInitial": false, "isFinal": false, "enabled": true },
        { "id": "DELIVERED", "isInitial": false, "isFinal": true, "enabled": true },
        { "id": "CANCELLED", "isInitial": false, "isFinal": true, "enabled": true }
      ],
      "actions": [
        { "id": "PROCESS_ORDER", "enabled": true, "fromStates": ["PENDING"], "toState": "PROCESSING" },
        { "id": "SHIP_ORDER", "enabled": true, "fromStates": ["PROCESSING"], "toState": "SHIPPED" },
        { "id": "DELIVER_ORDER", "enabled": true, "fromStates": ["SHIPPED"], "toState": "DELIVERED" },
        { "id": "CANCEL_ORDER_PENDING", "enabled": true, "fromStates": ["PENDING"], "toState": "CANCELLED" },
        { "id": "CANCEL_ORDER_PROCESSING", "enabled": true, "fromStates": ["PROCESSING"], "toState": "CANCELLED" }
      ]
    }
    ```

### 2. Start Workflow Instance

* **Endpoint**: `POST /workflows/instances`
* **Description**: Starts a new instance of a given workflow definition.
* **Request Body Example**: (Just the definition ID as a string)
    ```json
    "OrderProcessing"
    ```

### 3. Execute Action on Instance

* **Endpoint**: `POST /workflows/instances/{instanceId}/execute/{actionId}`
* **Description**: Executes an action on a specific workflow instance, moving its state if valid.
* **Example URL**: `/workflows/instances/YOUR_INSTANCE_ID/execute/PROCESS_ORDER`

### 4. Get Workflow Instance Details

* **Endpoint**: `GET /workflows/instances/{instanceId}`
* **Description**: Retrieves the current state and history of a workflow instance.

### 5. List All Workflow Definitions

* **Endpoint**: `GET /workflows/definitions`
* **Description**: Lists all defined workflows.

### 6. List All Workflow Instances

* **Endpoint**: `GET /workflows/instances`
* **Description**: Lists all currently running workflow instances.

## Assumptions & Limitations

* **In-Memory Persistence**: All data (definitions and instances) is stored in memory and will be lost when the application restarts.
* **No Authentication/Authorization**: No security measures are implemented for API access.
* **Basic Error Handling**: Error messages are provided for invalid operations, but more detailed/structured error responses could be implemented.
* **No Update/Delete Operations**: The API currently only supports creating and retrieving definitions/instances, and executing actions. Update/delete functionality is not implemented.

## Future Enhancements (TODOs)

* Implementing persistent storage (e.g., using SQLite ).
* Adding more sophisticated logging.
* Implementing PATCH/PUT/DELETE operations for definitions and instances.
* Adding unit/integration tests for API endpoints and service logic.
* Implementing advanced validation (e.g., regex for IDs, more complex state rules).
* Adding OpenAPI documentation (Swagger) descriptions and examples for clearer API usage.