Municipal Citizen Issue Reporting System Overview

The Municipal Citizen Issue Reporting System is an ASP.NET MVC web application designed to empower citizens to actively engage with their municipality. The system allows users to report local issues, view local events and announcements, and in the final part of the project, track the Service Request Status for reported issues. Citizens can report issues such as sanitation problems, damaged roads, or utility faults by providing location, category, and description details, and attaching relevant images or documents. Administrators can manage these reports, update statuses through Read, In Progress, Request Sent, or Resolved, and escalate issues into service requests. When a service request is completed, the associated issue’s status is automatically updated to Resolved, maintaining a smooth and connected workflow. The Local Events & Announcements module allows citizens to view recent, upcoming, and personalized recommended events based on their interactions and preferences. Administrators can create events, upload files, and monitor citizen engagement. The system leverages MongoDB Atlas for persistent data storage and GridFS for managing media attachments such as images and documents. The MVC architecture separates concerns between the citizen-facing interface and administrative control panels, providing a clean and maintainable code structure.

Packages and setups

Install Visual Studio 2019 or 2022 with the ASP.NET and web development workload.

Create a MongoDB Atlas account and configure a cluster with a connection string.

Ensure the following NuGet packages are installed:

MongoDB.Driver

MongoDB.Driver.GridFS

System.Collections.Generic (for dictionaries, sets, and custom collections)

More Setup Steps

Clone or download the project to your local machine.

Open the solution file in Visual Studio.

Update the MongoDB connection string in appsettings.json or web.config.

Build and Run

Build the project via Ctrl + Shift + B or Build → Build Solution.

Set the MVC project as the startup project.

Press F5 or Debug → Start Debugging.

The Main Menu page will load, allowing citizens to report issues or browse events.

Key Features Report Issues

Location Input: Textbox for issue location.

Category Selection: Dropdown/ListBox with categories (tracked via sets for unique category management).

Description Box: RichTextBox for detailed issue description.

File Attachments: Users can upload images/documents stored securely in GridFS.

Status Workflow: Issues can be marked as Read, In Progress, Request Sent, or Resolved. Escalation to service requests is handled automatically.

Local Events & Announcements

Citizens can browse Recent, Upcoming, and Recommended Events.

Administrators can create, edit, and manage events, uploading associated media files.

Recommendations are personalized using priority queues, sets, and dictionaries to track citizen interaction history and preferences.

Service Request Status

Displays the progress of issues escalated to service requests.

Admins can update the status of requests, which also updates the corresponding issue automatically.

Ensures a seamless connection between issue reporting and municipal service resolution.

Data Structures and Purpose

Dictionaries & Custom Dictionaries: Store issues, events, and user preferences for fast lookup and caching before saving to MongoDB. For example, when a citizen reports a “Water Leakage” issue, the application stores this report in a Dictionary<int, ServiceRequest> where the key is the unique issue ID, and the value is the corresponding service request object. This allows the admin to instantly retrieve and update the request’s status in real time complexity, improving the efficiency of the “Service Request Status” feature compared to a full list traversal.

Sets: Track unique categories and citizen interactions for recommendations, preventing duplicates and improving recommendation efficiency. A HashSet and a custom categoryset datastructure is utilized to store unique service request IDs, ensuring that duplicate requests are never processed twice. For example, if two users attempt to report the same pothole location, the hash set ensures that only one entry is maintained in the database, improving data consistency and preventing redundant updates.

Priority Queues: Organize events by recent, upcoming, or recommended status for efficient retrieval and display. The priority queue is used to ensure urgent service requests (such as power outages or road blockages) are handled before lower-priority ones. For instance, when multiple reports are submitted, the system inserts each into a PriorityQueue<ServiceRequest, int> where the priority is determined by urgency level. This means a report marked as “Critical” will automatically be dequeued and addressed before “Moderate” or “Low” requests, ensuring fair and efficient service scheduling.

Custom Lists: Temporarily cache objects (issues or events) before persisting them in MongoDB, improving performance and reducing database calls. These structures ensure efficient data handling, quick lookups, and optimized recommendation generation, improving both user experience and application performance.

The Binary Search Tree (BST) structure is used to store and retrieve service requests in a sorted order based on their creation date or priority. For instance, when an admin wants to view pending requests in ascending order of submission date, an in-order traversal of the BST provides a naturally ordered list, which enhances both search and filtering performance within the “Service Request Status” module.

The Graph data structure is implemented to model and analyze the relationships between various municipal service points. Each node represents a location (such as a neighborhood or district), and edges represent routes or service connections between them. For example, when a service truck needs to travel between two service locations, the graph allows the system to determine the shortest path or identify all connected areas affected by the same service request. This makes the system more efficient by avoiding redundant checks across unrelated areas.

Application Flow

Citizen opens the application and sees the Main Menu.

Selecting Report Issues allows users to submit issue details and attachments.

Administrators access the All Issues page to review and update statuses.

When an issue is escalated to a service request, it is tracked in the Service Request Status page. Completion of the service request automatically updates the original issue to Resolved.

Citizens can view Local Events & Announcements, including personalized recommendations on the Dashboard, while admins manage events separately.

Usage

Report Issues: Citizens enter location, category, description, attach files → submit → saved to MongoDB.

Browse Events: Citizens can view Recent, Upcoming, and Recommended events. Admins create/manage events with attachments.

Track Service Requests: Citizens see status updates, while admins can update Service Requests → automatically updates original issues.

Application Flow

Citizen workflow: Main Menu - Report Issues - submit details and attachments - optionally browse Local Events & Announcements - view recommendations.

Administrator workflow: Admin dashboard - view All Issues - update statuses (Read, In Progress, Request Sent, Resolved) - manage Service Requests - create/manage events - monitor citizen engagement.

Service Request Status is linked directly with issues, so updates to service requests automatically update the original issue.

Roles

Citizen User:

Report issues with attachments.

View local events and announcements.

See recommended events based on previous interactions.

Administrator:

Manage reported issues and service request statuses.

Create and manage events.

Monitor citizen engagement and recommendations.

How to Use Reporting Issues

Launch the application.

Select Report Issues.

Fill in location, category, description, and attach files if needed.

Click Submit to save the report to MongoDB.

Browsing Events

Citizens can access Local Events & Announcements to filter and view events.

Admins can create new events with details and attachments.

Recommended events are shown based on interaction history using sets and priority queues.

Tracking Service Requests

Citizens view the Service Request Status page to track escalated issues.

Admins update service request progress, which also updates the original issue automatically.

Refereces

Bhaskar, S. & Kaarsgaard, R., 2021. Graph Traversals as Universal Constructions. Available at: https://arxiv.org/abs/2104.14877 (Accessed: 12 November 2025).

Chugh, A., 2022. Binary Search Tree (BST) – Search, Insert and Remove. DigitalOcean Community Tutorials, 4 August. Available at: https://www.digitalocean.com/community/tutorials/binary-search-tree-bst-search-insert-remove (Accessed: 12 November 2025).

Gómez-Martínez, M., Cervantes-Ojeda, J. & García-Nájera, A., 2021. Association and Aggregation Class Relationships: is there a Difference in Terms of Implementation? In: 9th International Conference on Software Engineering Research & Innovation (CONISOFT). pp. 10-18. DOI: 10.1109/CONISOFT52520.2021.00018.

Kondratev, D., 2023. Unlocking the Potential of Binary Search Trees with C# Programming. DZone. Available at: https://dzone.com/articles/unlocking-the-potential-of-binary-search-trees-wit (Accessed: 12 Nov 2025).

Lorenzen, A., Leijen, D., Swierstra, W. & Lindley, S., 2024. The Functional Essence of Imperative Binary Search Trees. Microsoft Research Technical Report MSR-TR-2023-28, 27 December. Available at: https://homepages.inf.ed.ac.uk/slindley/papers/fiptree.pdf (Accessed: 12 November 2025).

Al-Fedaghi, S., 2022. Conceptual Modeling of Aggregation: an Exploration. arXiv preprint arXiv:2208.11171. Available at: https://arxiv.org/abs/2208.11171 (Accessed: 12 Nov 2025).
