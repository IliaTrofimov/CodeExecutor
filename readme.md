# Code Executor

[![Build and test .NET](https://github.com/IliaTrofimov/CodeExecutor/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/IliaTrofimov/CodeExecutor/actions/workflows/build-and-test.yml) [![CodeQL](https://github.com/IliaTrofimov/CodeExecutor/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/IliaTrofimov/CodeExecutor/actions/workflows/github-code-scanning/codeql)

ASP.NET demo project for remote code executing.

---

### Components architecture
Using ASP.NET (.net 8) for backend, and React for frontend part.
* **CodeExecutor.Auth** (url: `host/auth/`) for managing user accounts and authentication/authorization with JWT. Depends only on database (Postgres) to store data.
* **CodeExecutor.Dispatcher** (urls: `host/codeExecutions/` - public usage, `host/codeExecutionsModification/` - internal usage) for managing code executions (start, view results/sources, delete from history). Depends on database (Postgres) and message queue (RabbitMQ) for communicating with code runners.
* **CSharpCodeExecutor** - the only code runner yet developed. Receives messages from RabbitMQ, validates source code and runs it. Results are being returned to **CodeExecutor.Dispatcher** at `host/codeExecutionsModification/`. 
* **CodeExecutor.UI** - user interface.


### Deployment and running
**Local**

To run project locally you must build and run desired C# projects (**CodeExecutor.Auth.Host**, **CodeExecutor.Dispatcher.Host** or **CSharpCodeExecutor**). See `appsettings.Development.json` to configure services' parameters. 

**Docker**

Use `docker-compose.yml` file to run project in Docker. `appsettings.Production.json` is configuring services, also parameters can be passed to services inside compose `environment` section.

**Connections**
* Use `localhost` to connect to all services when running locally.
* Use container name (e.g. `rabbitmq` or `code.executor.dispatcher:80`) to connect services inside Docker.
* Use container port to connect C# services inside Docker (`code.executor.dispatcher:80` not `code.executor.dispatcher:5030`).
