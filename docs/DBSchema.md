# AI-Based LMS Database Schema

---

## Users (TPH - Table Per Hierarchy)

* `user_id` (PK)
* `username` (unique, indexed)
* `password_hash`
* `full_name`
* `user_type` (enum: 'student', 'instructor', 'admin')
* `phone`
* `profile_picture_url` (nullable)
* `created_at` (datetime)
* `created_by` (FK → Users.user_id)

**Student-specific columns (nullable):**
* `student_id_number` (Unique, indexed)
* `sent_via_notification_email` (boolean, default = true)

---

## Courses

* `course_id` (PK)
* `course_code` (unique, indexed)
* `course_name`
* `description` (text, nullable)
* `instructor_id` (FK → Users.user_id)
* `status` (enum: 'pending', 'approved', 'rejected', default = 'pending')
* `approval_date` (datetime, nullable)
* `approved_by` (FK → Users.user_id, nullable)
* `created_at` (datetime)
* `section_course_id` (FK → Courses.course_id)

---

## Enrollments

* `(student_id, course_id)` (PK)
* `student_id` (PK, FK → Users.user_id)
* `course_id` (PK, FK → Courses.course_id)
* `request_date` (datetime)
* `approval_date` (datetime, nullable)
* `approved_by` (FK → Users.user_id, nullable)

---

## CourseMaterials

* `material_id` (PK)
* `course_id` (FK → Courses.course_id)
* `lecture_id` (FK → Lectures.lecture_id, nullable)
* `title`
* `description` (text, nullable)
* `material_type` (enum: 'video', 'pdf', 'document', 'presentation', 'other')
* `file_url`
* `upload_date` (datetime)
* `uploaded_by` (FK → Users.user_id)
* `order_index` (integer, default = 0)

---

## Lectures

* `lecture_id` (PK)
* `course_id` (FK → Courses.course_id)
* `title`
* `description` (text, nullable)
* `lecture_number` (integer)

---

## Assignments

* `assignment_id` (PK)
* `course_id` (FK → Courses.course_id)
* `title`
* `description` (text)
* `due_date` (datetime)
* `allow_late_submission` (boolean, default = false)
* `allowed_sumbmission_number` (integer, default = 1)
* `created_by` (FK → Users.user_id)
* `created_at` (datetime)

---

## AssignmentSubmissions

* `submission_id` (PK)
* `assignment_id` (FK → Assignments.assignment_id)
* `student_id` (FK → Users.user_id)
* `submission_date` (datetime)
* `file_url`
* `is_late` (boolean)
* `feedback` (text, nullable)

---

## Quizzes

* `quiz_id` (PK)
* `course_id` (FK → Courses.course_id)
* `title`
* `description` (text, nullable)
* `total_marks` (decimal or integer)
* `duration_minutes` (integer)
* `is_live` (boolean)
* `start_time` (datetime, for live quizzes)
* `end_time` (datetime, for offline - availability period)
* `is_published` (boolean, default = false)
* `published_date` (boolean, default = false)
* `allow_multiple_attempts` (boolean, default = false)
* `show_results_immediately` (boolean, default = false)
* `created_by` (FK → Users.user_id)
* `created_at` (datetime)

---

## QuizQuestions

* `question_id` (PK)
* `quiz_id` (FK → Quizzes.quiz_id)
* `question_text` (text)
* `question_type` (enum: 'mcq', 'true_false', 'written', 'true_false_with_correction')
* `marks` (decimal or integer)
* `order_index` (integer)

---

## QuestionOptions

* (`option_id`, `question_id`) (PK)
* `question_id` (FK → QuizQuestions.question_id)
* `option_text` (text)
* `is_correct` (boolean, default = false)
* `order_index` (integer)

---

## QuizAttempts

* `attempt_id` (PK)
* `quiz_id` (FK → Quizzes.quiz_id)
* `student_id` (FK → Users.user_id)
* `start_time` (datetime)
* `end_time` (datetime, nullable)
* `score` (decimal or integer, nullable)
* `total_marks` (decimal or integer)
* `status` (enum: 'in_progress', 'submitted', 'graded', default = 'in_progress')
* `attempt_number` (integer)

---

## QuizAnswers

* `answer_id` (PK)
* `attempt_id` (FK → QuizAttempts.attempt_id)
* `question_id` (FK → QuizQuestions.question_id)
* `selected_option_id` (FK → QuizOptions.option_id, nullable)
* `text_answer` (text, nullable - for short answer/essay)
* `is_correct` (boolean, nullable - auto-graded for MCQ/True-False)
* `marks_awarded` (decimal or integer, nullable)

---

## Notifications

* `notification_id` (PK)
* `user_id` (FK → Users.user_id)
* `title`
* `message` (text)
* `notification_type` (enum: 'new_material', 'assignment', 'quiz', 'announcement', 'enrollment', 'grade')
* `related_entity_type` (enum: 'course', 'assignment', 'quiz', 'material', nullable)
* `related_entity_id` (integer, nullable)
* `is_read` (boolean, default = false)
* `created_at` (datetime)

---

## Announcements

* `announcement_id` (PK)
* `course_id` (FK → Courses.course_id)
* `title`
* `content` (text)
* `created_by` (FK → Users.user_id)
* `is_pinned` (boolean, default = false)
* `created_at` (datetime)
