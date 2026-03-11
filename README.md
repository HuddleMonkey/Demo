# Demo Web API
This repository is a Web API developed using .NET 10, representing a small extract of code currently running in production that highlights authentication and a segment of event management. The solution consists of four projects, outlined below.

| Project | Description |
| ----------- | ----------- |
| Demo.Api | Contains the controllers, middleware, and framework for the Web API |
| Demo.Application | Contains the heart of the application using clean architecture and the Command-Query Responsibility Segregation (CQRS) and Vertical Slice Architecture (VSA) patterns. |
| Demo.Dto | Contains the Data Transfer Objects (DTO) the Web API uses for incoming requests and to return data to the calling client. |
| Demo.Shared | Contains shared code, such as constants and utilities, used in Web API and any clients |

## Demo.Application Overview
The Demo.Application project is logically separated using the clean architecture pattern. While some solutions split the different layers into separate projects, the design decision was made to keep the layers in one project with logical separation with folder structures. This allowed for code in the same domain to remain close to each other while still adhering to the clean architecture pattern. All communication between domains is done through CQRS, implemented with MediatR.

### Domain
This folder contains the base and common domain entities for the application.

### Features
This folder contains the Vertical Slices of each self-contained domain, such as Authentication, Events, and Users. Within each feature, you will find up to six folders.

| Folder | Description |
| ----------- | ----------- |
| Commands | The commands in the Command-Query Responsibility Segregation (CQRS) pattern |
| Infrastructure | The infrastructure layer in the clean architecture that contains any infrastructure, such as database implementations and calls to Azure, related to that specific domain. |
| Interfaces | Any interfaces needed for the domain. |
| Models | Models that define the domain. |
| Notifications | The notifications published in the Command-Query Responsibility Segregation (CQRS) pattern |
| Queries | The queries in the Command-Query Responsibility Segregation (CQRS) pattern |

### Infrastructure
This folder contains the base infrastructure needed for the application, such as the database context. Actual implementations of the infrastructure are defined under the features for each vertical slice.

## Authentication
As a SaaS application, users are either part of an organization and/or a content provider (they provide training, videos, or other content). To successfully authenticate, a user must have a valid account - either a member of an active organization or a provider. A user can be associated with more than one organization.

Users can only access content in the organization they are associated with, and even within an organization they can have different permissions based on their role.
