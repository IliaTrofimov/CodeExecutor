# Code Executor

[![Build and test .NET](https://github.com/IliaTrofimov/CodeExecutor/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/IliaTrofimov/CodeExecutor/actions/workflows/build-and-test.yml) [![CodeQL](https://github.com/IliaTrofimov/CodeExecutor/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/IliaTrofimov/CodeExecutor/actions/workflows/github-code-scanning/codeql)

ASP.NET demo project for remote code executing.

---

### Components architecture
Using ASP.NET (.net 8) for backend, and React for frontend part.
* **CodeExecutor.Auth** (url: `your_host/auth/`) for managing user accounts and authentication/authorization with JWT. Depends only on database (Postgres).
* **CodeExecutor.Dispatcher** (urls: `your_host/codeExecutions/` - for users, `your_host/codeExecutionsModification/` - for internal usage) for managing code executions (start, view results/sources, delete from history). Depends on database (Postgres) and message queue (RabbitMQ) for communicating with code runners.
* **CodeExecutor.UI** - user interface.
* **CSharp12Executor** - the only code runner yet developed. Receives messages from RabbitMQ, validates source code and runs it. Results are being returned to **CodeExecutor.Dispatcher** at `your_host/codeExecutionsModification/`.