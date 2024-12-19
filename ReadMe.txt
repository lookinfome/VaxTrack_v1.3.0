# Vax Track v1.2.0

## New Addition
- Support Module: to raise ticket and add follow-up comments - completed
- Support Management Module: to update/modify/manage/close tickets raised by user - work in progress

## Updates (refer 'notes' section for more)
- UI Update: Home Page - completed
- UI Update: SignIn Page - completed
- UI Update: SignUp Page - completed
- UI Update: User Profile Page - completed
- UI Update: User Edit Profile Page - completed
- UI Update: Slot Book Page - completed
- UI Update: Admin Page - completed
- UI Update: Add Suitable Icons - completed

## Fixes
- NA

## Identified Bugs
-- Bug: user edit profile modal - closing confirm password modal freezing the site page
-- Bug: admin dashboard font color - post updating Bootstrap to v5.3 overriding css to default - completed
-- Bug: application font - post updating Bootstrap to v5.3 excluded google font - completed
-- Bug: hospital slot updates - post adding support manager tab on admin dashboard, slot is not updating - completed
-- Bug: toggling active tabs - post adding support manager tab on admin dashboard, toggling is not consistent - completed 
-- Bug: toggling slot book buttons - post slot booking messages and buttons are not consistent while getting displayed - completed


## Notes
- removed independent login page
- removed independent registration page
- removed independent user profile edit page 
- removed independent slot booking page 
- removed v1 admin page
- added partial login view on home page
- added partial registration view on home page
- added user profile edit modal on user profile page    : scrapped before final deployment
- added user profile edit inline buttons on user profile page
- added slot booking modal on user profile page
- added v2 admin page with different tabs promoting seperation and UI friendliness
- added support module with Support View and Controller
- replaced alert messages to toast messages
- performed css cleanup 