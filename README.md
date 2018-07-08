# leo.v2

Leo is a personal finance app I use in spare time to expirement with new technologies. This version started in 2018 and is a work in progress to demonstrate an open source project collaboration between myself and the service fabric product team. 

The pool manager managers a buffer of stateful service instances that can be dynamically allocated for a unit of work then disposed/released back into the pool. The pattern is similar to virtual actors. However, this allows the actors to be built with the flexibility and reliablity of Stateful Reliable Serivces. 

## Technologies
 - Azure Service Fabric 
 -- Stateful Reliable Services
 - Service Fabric Pool Manager (open-source project currently private)
 - Angular 6
 - Web API 

## Patterns
 - CQRS
 - Event Sourcing
 - Domain Driven Design (DDD)
 - Multi-Region/Multi-Cluster Active/Active (coming soon!)
 - Microservices
 - Stateful Services
 - High Availablility

## Third-Party Integrations
 - [Plaid](https://plaid.com/) (coming soon!)
 
