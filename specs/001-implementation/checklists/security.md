# Security Checklist: Implementation

**Purpose**: Verify security requirements and best practices
**Created**: October 19, 2025
**Feature**: [../spec.md](../spec.md)

## Authentication & Authorization

- [ ] CHK001 Authentication properly implemented
- [ ] CHK002 Authorization checks in place
- [ ] CHK003 Session management secure
- [ ] CHK004 Password policies enforced

## Data Protection

- [ ] CHK005 Sensitive data encrypted at rest
- [ ] CHK006 Secure transport (HTTPS/TLS)
- [ ] CHK007 Input sanitization implemented
- [ ] CHK008 Output encoding in place

## Infrastructure

- [ ] CHK009 Security headers configured
- [ ] CHK010 Error handling doesn't leak info
- [ ] CHK011 Logging excludes sensitive data
- [ ] CHK012 Dependencies are up to date

## Notes

- Check items off as completed: `[x]`
- Document security decisions
- Note any security exceptions
- Include links to security reviews