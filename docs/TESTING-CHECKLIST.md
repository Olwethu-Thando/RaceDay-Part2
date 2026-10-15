# Swagger verification checklist

1. Register Organiser → 201; duplicate email → 409; invalid role → 400.
2. Log in Organiser → 200 and cookie. Wrong password → 401.
3. Create Event → 201. Create Category → 201. Get upcoming Events → 200.
4. Log out and register/log in Participant. Create Event → 403; Enrol in Event → 201; duplicate → 409.
5. Log out and log in Organiser. View owned event enrolments → 200; confirm enrolment → 200.
6. For results, create a past event and a confirmed enrolment (via test fixture or a test-only database setup), then capture results → 201; participant sees own result and public leaderboard shows it.
7. Access /api/users/me after logout → 401. Different Organiser editing event → 403.
8. Check DB tables in SSMS, run `dotnet test RaceDay.sln`, and check green GitHub Actions run.
