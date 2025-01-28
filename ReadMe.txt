# Vax Track v1.3.0

## New Addition
- Profile Pic: to upload profile pic - completed
- Password Reset Feature: to reset password - completed
- Support Module: to raise ticket and add follow-up comments - completed
- Support Management Module: to update/modify/manage/close tickets raised by user - partially completed (refer section - leftovers)

## Updates (refer 'update notes' section for more)
- UI Update: Home Page - completed
- UI Update: SignIn Page - completed
- UI Update: SignUp Page - completed
- UI Update: User Profile Page - completed
- UI Update: User Edit Profile Page - completed
- UI Update: Slot Book Page - completed
- UI Update: Admin Page - completed
- UI Update: Add Suitable Icons - completed

## Identified Bugs
-- Bug: user edit profile modal - closing confirm password modal freezing the site page
-- Bug: admin dashboard font color - post updating Bootstrap to v5.3 overriding css to default - completed
-- Bug: application font - post updating Bootstrap to v5.3 excluded google font - completed
-- Bug: hospital slot updates - post adding support manager tab on admin dashboard, slot is not updating - completed
-- Bug: toggling active tabs - post adding support manager tab on admin dashboard, toggling is not consistent - completed 
-- Bug: toggling slot book buttons - post slot booking messages and buttons are not consistent while getting displayed - completed


## Update Notes
- removed independent login page
- removed independent registration page
- removed independent user profile edit page 
- removed independent slot booking page 
- removed v1 admin page
- added partial login view on home page
- added partial registration view on home page
- added partial password reset view on home page
- added user profile edit modal on user profile page    : scrapped before final deployment (action by: LookInfoMe)
- added user profile edit inline buttons on user profile page
- added slot booking modal on user profile page
- added v2 admin page with different tabs promoting seperation and UI friendliness
- added support module with Support View and Controller
- replaced alert messages to toast messages
- performed css cleanup 

## Leftovers Points
- Support Module/Support Manager Module - files/media upload feature
- Support Manager Module - ticket status update
- Support Manager Module - SLA and OLA tracker
- Notification Module - for updates on ticket by either side (end users or admins)
- Code Cleanup - redundant codes in support and support manager services : typical dev probs, I was too lazy atm ;)

### Leftovers Points Additional:    For me it was for demo purpose but if someone wants to scale up the project, 
                                    You might need to first refine codes on below points,
- Exception Handling - need to handle in all controllers (I have done it but only on selective methods)
- Asynchronization - need to check and implement (I have done it but only on selective methods)
- Dependency on 3rd Party Libraries - to avoid processing time if requires, also ensure security if library is in-built by MS.
- Use Queue with Batch - I haven't used, but in case if requires to interact with Database with API call for bulk data/records (reduce processing time).

## Fixes
- NA
