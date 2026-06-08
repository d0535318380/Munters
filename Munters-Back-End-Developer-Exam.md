# Munters Back-End C# Developer Exam

**Allotted time:** 3 hours

## Giphy Integration

Create a simple application that will fetch data from [giphy.com](https://giphy.com).
The application will provide the following capabilities:

* Fetch the URLs of each trending GIF of the day.
* Search & Fetch the URLs of each GIF given a search term as input.
* Cache previous requests to prevent redundant API calls to the Giphy API.

To achieve these capabilities you should use the Giphy developer
API ([https://developers.giphy.com/](https://developers.giphy.com/)).

## Instructions

The application should be written in .NET Core and have two fetch methods which will be available via HTTP.
One to fetch the URLs of the trending GIFs of the day and another that receives a search term as input, and will return
the URLs of each GIF found.

Since the Giphy API is not free, and each request costs us X$ - you should implement a caching mechanism that will not
call the Giphy API for previously searched terms.

Make use of basic software engineering principles such as polymorphism, design patterns, SOLID principles. Make sure the
application is extendable and is easily adjusted to changes.

Additionally, the application should work as fast as possible, meaning there should be safe concurrent operations where
needed.

## Bonus

Create a UI that will be used as the frontend to the API you created, and will display the returned GIFs upon response.

You may use any UI technology you want.

## Review Criteria

1. Functional requirements completeness
2. Clean code and maintainability
    * Formatting, conventions, readability, etc.
3. Code design and best practices
4. Additional features
5. Design
6. This is understandably just an exam, it doesn’t have to be perfect and production ready. If you leave things at a
   certain point where you know it can be improved further, do write a comment indicating so and how

* It’s recommended to focus on completing the core tasks over implementing smart solutions / making use of advanced
  features.

**Good luck**
