// Generated read-only source records by scripts/sync-school-workspaces.cjs
namespace Rynzo.Client.Features.School.Models;
public static class SchoolSource { public const string Json = """
{
  "classes": [
    {
      "id": "8-A",
      "title": "Grade 8 / A",
      "teacher": "Sophia Carter",
      "room": "Room 201",
      "capacity": 30,
      "subjects": 7,
      "color": "primary"
    },
    {
      "id": "9-A",
      "title": "Grade 9 / A",
      "teacher": "Daniel Reyes",
      "room": "Room 202",
      "capacity": 32,
      "subjects": 8,
      "color": "info"
    },
    {
      "id": "10-A",
      "title": "Grade 10 / A",
      "teacher": "Emma Wilson",
      "room": "Room 301",
      "capacity": 30,
      "subjects": 8,
      "color": "success"
    },
    {
      "id": "11-A",
      "title": "Grade 11 / A",
      "teacher": "Noah Williams",
      "room": "Room 302",
      "capacity": 28,
      "subjects": 6,
      "color": "warning"
    },
    {
      "id": "12-A",
      "title": "Grade 12 / A",
      "teacher": "Olivia Smith",
      "room": "Room 401",
      "capacity": 28,
      "subjects": 6,
      "color": "primary"
    }
  ],
  "students": [
    {
      "id": "STU-101",
      "name": "Ava Thompson",
      "classId": "8-A",
      "guardian": "Sarah Thompson",
      "email": "sarah@example.com",
      "status": "Active",
      "photo": "assets/images/users/user-1.jpg"
    },
    {
      "id": "STU-102",
      "name": "Liam Brown",
      "classId": "8-A",
      "guardian": "Michael Brown",
      "email": "michael@example.com",
      "status": "Active",
      "photo": "assets/images/users/user-2.jpg"
    },
    {
      "id": "STU-103",
      "name": "Emma Davis",
      "classId": "9-A",
      "guardian": "Laura Davis",
      "email": "laura@example.com",
      "status": "Active",
      "photo": "assets/images/users/user-3.jpg"
    },
    {
      "id": "STU-104",
      "name": "Noah Patel",
      "classId": "9-A",
      "guardian": "Priya Patel",
      "email": "priya@example.com",
      "status": "On leave",
      "photo": "assets/images/users/user-4.jpg"
    },
    {
      "id": "STU-105",
      "name": "Olivia Martin",
      "classId": "10-A",
      "guardian": "Alex Martin",
      "email": "alex@example.com",
      "status": "Active",
      "photo": "assets/images/users/user-1.jpg"
    },
    {
      "id": "STU-106",
      "name": "James Wilson",
      "classId": "10-A",
      "guardian": "Grace Wilson",
      "email": "grace@example.com",
      "status": "Active",
      "photo": "assets/images/users/user-2.jpg"
    },
    {
      "id": "STU-107",
      "name": "Sophia Lee",
      "classId": "11-A",
      "guardian": "David Lee",
      "email": "david@example.com",
      "status": "Active",
      "photo": "assets/images/users/user-3.jpg"
    },
    {
      "id": "STU-108",
      "name": "Ethan Miller",
      "classId": "11-A",
      "guardian": "Rachel Miller",
      "email": "rachel@example.com",
      "status": "Active",
      "photo": "assets/images/users/user-4.jpg"
    },
    {
      "id": "STU-109",
      "name": "Mia Anderson",
      "classId": "12-A",
      "guardian": "Lucy Anderson",
      "email": "lucy@example.com",
      "status": "Active",
      "photo": "assets/images/users/user-1.jpg"
    },
    {
      "id": "STU-110",
      "name": "Lucas Meyer",
      "classId": "12-A",
      "guardian": "Thomas Meyer",
      "email": "thomas@example.com",
      "status": "Active",
      "photo": "assets/images/users/user-2.jpg"
    }
  ],
  "exams": [
    {
      "id": "EXM-101",
      "subject": "Mathematics",
      "classId": "10-A",
      "date": "2026-10-12",
      "time": "09:00",
      "duration": 90,
      "room": "Hall A",
      "status": "Scheduled"
    },
    {
      "id": "EXM-102",
      "subject": "English",
      "classId": "9-A",
      "date": "2026-10-13",
      "time": "10:00",
      "duration": 60,
      "room": "Hall B",
      "status": "Scheduled"
    },
    {
      "id": "EXM-103",
      "subject": "Science",
      "classId": "8-A",
      "date": "2026-10-14",
      "time": "09:00",
      "duration": 90,
      "room": "Lab 1",
      "status": "Scheduled"
    },
    {
      "id": "EXM-104",
      "subject": "Physics",
      "classId": "11-A",
      "date": "2026-10-15",
      "time": "10:00",
      "duration": 120,
      "room": "Hall A",
      "status": "Scheduled"
    },
    {
      "id": "EXM-105",
      "subject": "Economics",
      "classId": "12-A",
      "date": "2026-10-16",
      "time": "09:00",
      "duration": 90,
      "room": "Hall B",
      "status": "Scheduled"
    },
    {
      "id": "EXM-106",
      "subject": "History",
      "classId": "10-A",
      "date": "2026-10-17",
      "time": "11:00",
      "duration": 60,
      "room": "Hall A",
      "status": "Cancelled"
    }
  ]
}
"""; }
