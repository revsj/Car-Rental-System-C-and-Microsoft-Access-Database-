ALL FORMS: 
Form 1 - Choosing between a customer or admin side 
Form 2 - validation of admin - typing passwords and username
Form 3 - Admin side 
Form 4 - Customer side 

Complete List of Features & Functions Built
Form1 — Main Selection Screen
Role selection between Admin and Customer
Navigation to Admin login (Form2) or Customer rental (Form4)
AutoScroll enabled for content overflow

Form2 — Admin Login
Username and password validation
Error message for wrong credentials
Redirects to Admin Dashboard (Form3) on success

Form3 — Admin Dashboard
Single grid that switches between Cars, Customers, and Rentals tabs
Tab switching — CAR DETAILS, CUSTOMER DETAILS, RENTAL DETAILS buttons
Left panel (MANAGE CARS) auto-hides when viewing Customers or Rentals
Grid expands full width when left panel is hidden
Latest records shown first (ORDER BY DESC) for Customers and Rentals
Add Car — inserts new car record with full validation before database insert
Update Car — edits selected car fields with validation
Delete Car — removes car with Yes/No confirmation dialog
Upload Images — up to 3 images per car using FileStream (avoids file locking)
Image path columns hidden from grid view
Row click auto-populates all textboxes and loads car images
Refresh button reloads current active tab

Clear button resets all fields, images, and background colors
Logout button returns to Form1
Grid styled with brown headers, MistyRose alternating rows, IndianRed selection

Form4 — Customer Dashboard
Car grid with image columns hidden
Row click displays car details (Brand, Model, Car ID, Rental Price, Status) in labels
Car images load into 3 PictureBoxes on row click
Date pickers — past dates disabled, return date minimum is always 1 day after start
Billing calculation — Base Price × Days + 12% VAT − 20% Senior/PWD discount
Double booking check — prevents renting a car already rented and not yet returned
Invoice display — shows full breakdown before confirmation
Yes/No confirmation — database only saves if user confirms
Two-step insert — Customer record first, then Rental linked via CustomerID
Auto-updates Car Status to "Rented" after confirmed rental
Grid refreshes after successful rental
Clear button resets all fields, date pickers, labels, and selected car variables

Validation (Both Forms) Form3 and Form4 
Contact Number — numbers only, blocked at input, max 11 digits, must start with 09
Agency No — numbers only, blocked at input
Full Name — letters only, auto-removes invalid characters
Email — validated on Leave event, must contain @
Rental Price — numbers and decimals only, real-time MistyRose background on error
Year — numbers only, blocked at input
Color — text only, numbers blocked
Brand — text only, numbers trigger MistyRose background
Empty field checks on all submit buttons
Status dropdown must be selected before Add
