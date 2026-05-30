# Security Policy

## Supported Versions

These version are supported versions which **may** get security updates.

| Version | Supported          |
| ------- | ------------------ |
| >= 1.0.2 | :white_check_mark: |
| < 1.0.2  | :x:                |

## Reporting a Vulnerability

We take the security of our tools and the ecosystems relying on them seriously. If you discover a vulnerability or a potential flaw in how F-Droid index signatures, hashes, or payload packages are parsed, please report it immediately.

### How to Report
Do **not** open a public GitHub issue for security vulnerabilities. Instead, please report any security concerns directly by:
* Opening a confidential report if you have private contact info for the project maintainers.
* Alternatively, if utilizing an open channel, request a secure, private communication method before disclosing details.

### What to Include in the Report
To help us triage and patch the issue quickly, please provide:
* A clear description of the vulnerability.
* A proof of concept (PoC), such as a malicious or malformed F-Droid index sample that triggers unexpected behavior (e.g., remote code execution, crash loops, or directory traversal).
* Details on the potential impact of the exploit.

### Our Response Timeline
* **Acknowledgment:** You can expect an initial response acknowledging receipt of your report within 48 to 72 hours.
* **Status Updates:** We will keep you updated at least once a week while working on a resolution.
* **Resolution:** If the vulnerability is accepted, we will coordinate a fix and release a patched version (e.g., a new minor or patch version release). We will also credit you for the discovery in the release notes if desired. If declined, we will provide a detailed technical rationale explaining why the behavior is expected or considered out of scope.
