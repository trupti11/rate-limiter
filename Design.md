# Design thoughts and considerations:
- The Rate Limiting Pattern is already readily available in the .NET framework (available from .NET 7.0 framework), 
However, since this is a coding exercise, wanted to demonstrate how I approach any new project opportunity.
- There are many architectural patterns to consider for this problem, clean architecture, microservice architecture, 
Domain Driven Design, API Gateway with plug in rules, etc. (there are definitely others that I might not have thought of).
- However, I want to demonstrate my skill to handle complex projects and a more methodical approach to problems.
- Although, an out of the box API Gateway with plug-ins would be the most simplistic approach, it will be best to actually demonstrate
my design skills using a Fast DDD API. 
- Also, DDD is probably an overkill for this coding exercise as a coding exercise is meant to be simple problem, 
I would still like to use this opportunity to design and implement a workable DDD Fast API.

# UML Diagrams (I wrote the content and let ChatGPT draw me the prettified diagrams)

## Context Diagram:
- API Client: Sends requests to the Rate Limiter.
- Rate Limiter: Evaluates the ruleset to determine if the request can proceed.
- API Resource: The protected resource that the client wants to access.
- Ruleset: Defines and configures the rate-limiting rules.
+-------------------+       +--------------------+       +-------------------+
|                   |       |                    |       |                   |
|    API Client     +------->    Rate Limiter    +------->    API Resource   |
|                   |       |                    |       |                   |
+-------------------+       +--------------------+       +-------------------+
                                 ^   |
                                 |   |
                            +-----------------+
                            |     Ruleset     |
                            +-----------------+

## Class Diagram:
The Class Diagram is the blueprint for the implementation in C#. Here’s how the diagram maps to the code:
- Client: This class holds metadata for API clients, including tokens and regions.
- RateLimiter: Implements the main logic for configuring and enforcing rate-limiting rules.
- Rule: Represented by the IRule interface.
- SpecificRule: Represented by individual rule classes like FixedRequestLimitRule and TimespanRule.
- CompositeRule: Combines multiple rules, ensuring flexibility and extensibility.
The class relationships (e.g., RateLimiter depends on IRule implementations) are directly reflected in the C# implementation.

Components:
- Client: Represents the API client, identified by a token and metadata (e.g., region).
- RateLimiter: Orchestrates rule configuration and enforcement.
- Rule: Abstract class for all rate-limiting rules.
- SpecificRule: Implementation of individual rules like "X requests per timespan".
- CompositeRule: Allows combining multiple rules for a resource.

+---------------------+
|       Client        |
+---------------------+
| - token: string     |
| - region: string    |
| - timestamp: Date   |
+---------------------+
          |
          v
+---------------------+
|     RateLimiter     |
+---------------------+
| - rules: List<Rule> |
+---------------------+
| + applyRules(): bool|
| + configureRules()  |
| + getLimits()       |
+---------------------+
          |
          v
+---------------------+        +----------------------+
|        Rule         |<-------+   CompositeRule      |
+---------------------+        +----------------------+
| + isAllowed(): bool |        | - rules: List<Rule>  |
+---------------------+        | + isAllowed(): bool  |
                                +----------------------+
          ^
          |
+---------------------+
|    SpecificRule     |
+---------------------+
| + isAllowed(): bool |
+---------------------+

## Sequence Diagram:
The Sequence Diagram details the flow of a single request, and the implementation follows this pattern step by step:
	1.	The Client sends a request: In C#, the Client object is created and passed to the RateLimiter.
	2.	The RateLimiter applies the configured rules: This is implemented in RateLimiter.IsRequestAllowed(Client, string).
	3.	The Rules are evaluated: Individual rule classes (FixedRequestLimitRule, TimespanRule, etc.) check their conditions.
	4.	If allowed, the request proceeds: The RateLimiter returns true if all rules pass, simulating the request proceeding to the resource.

Client               RateLimiter           Rule/CompositeRule         API Resource
   |                       |                        |                      |
   |   Send Request        |                        |                      |
   |---------------------->|                        |                      |
   |                       | Apply Rules            |                      |
   |                       |----------------------->|                      |
   |                       |  Check Rule 1          |                      |
   |                       |<-----------------------|                      |
   |                       |  Check Rule 2          |                      |
   |                       |<-----------------------|                      |
   |                       | Result: Allowed/Denied |                      |
   |                       |                        |                      |
   | Result                |                        |                      |
   |<----------------------|                        |                      |
   |                       |                        |                      |
   | If Allowed, Forward   |                        |                      |
   |---------------------->|----------------------->|                      |
   |                       |                        | Access Resource      |
   |                       |                        |--------------------->|

## Data Model Diagram:
The Data Model Diagram reflects how data (e.g., clients, rules, and their configurations) is stored in the application:
- Client maps to the Client class, storing metadata like tokens and timestamps.
- Rule is represented by the IRule interface and its implementations.
- RateLimiter holds resource-to-rule mappings in the _resourceRules dictionary.
For example, the in-memory ConcurrentDictionary in FixedRequestLimitRule closely matches the idea of storing client request logs for rate evaluation.

+-------------------+       +------------------+       +--------------------+
|      Client       |       |      Rule        |       |    Rate Limiter    |
+-------------------+       +------------------+       +--------------------+
| - Token: String   |       | - Name: String   |       | - Resource: String |
| - Region: String  |       | - Limit: Integer |       | - Rules: List<Rule>|
| - LastAccess: Date|       | - Timespan: Time |       |                    |
+-------------------+       +------------------+       +--------------------+

## Rule Composition Diagram:
The Rule Composition Diagram demonstrates how individual rules can be combined into a CompositeRule for flexibility.
- This is implemented via the CompositeRule class in C#.
- The CompositeRule aggregates multiple rules and ensures that all of them must pass for a request to be allowed.
- Rules like FixedRequestLimitRule and TimespanRule can be individually configured and combined as needed.
For example:
var compositeRule = new CompositeRule(new[] { fixedLimitRule, timespanRule });
rateLimiter.ConfigureRules("ResourceA", compositeRule);
This matches the diagram where rules like “X requests per timespan” and “Timespan-based rule” are combined into a composite unit.
Integration of Diagrams in Implementation
The design diagrams were critical in creating a structured and modular implementation:
	1.	High-Level Interaction (Context Diagram): Guided the overall structure of how components (Client, RateLimiter, Ruleset) interact.
	2.	Class Relationships (Class Diagram): Provided a clear blueprint for defining the relationships between classes (e.g., RateLimiter depends on IRule, rules inherit from IRule).
	3.	Request Flow (Sequence Diagram): Dictated the flow of logic in the RateLimiter.IsRequestAllowed method.
	4.	Data Storage (Data Model Diagram): Helped decide how to store and manage rate-limiting data in memory.
	5.	Rule Composition (Rule Composition Diagram): Inspired the CompositeRule implementation, enabling flexible rule combinations.
This alignment between the diagrams and the implementation ensures the design is consistent, modular, and extensible. If you’d like to see specific enhancements or further details, let me know!
+------------------+       +------------------+       +------------------+
|     Rule A       |       |     Rule B       |       |     Rule C       |
| - X requests/time|       | - Timespan-based |       | - Region-specific|
+------------------+       +------------------+       +------------------+


# An elaborate code folder structure for the DDD approach:
RateLimiterAPI/
├── Application/
│   ├── Interfaces/
│   ├── Services/
├── Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Rules/
│   ├── Aggregates/
├── Infrastructure/
│   ├── Repositories/
│   ├── Persistence/
│   ├── Middleware/
├── Presentation/
│   ├── Controllers/
├── API/
│   ├── Program.cs
│   ├── appsettings.json

