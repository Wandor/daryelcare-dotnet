-- Seed data: 11 applications from the dashboard hardcoded data

INSERT INTO applications (id, first_name, last_name, email, phone, dob, stage, start_date, registration_date, registration_number, last_updated, risk, progress, premises_type, premises_address, local_authority, registers, checks, connected_persons, ofsted_check)
VALUES
-- 1. Rebecca Morrison - REGISTERED
('RK-2024-00098', 'Rebecca', 'Morrison', 'rebecca.morrison@email.com', '07712 345678', '1985-03-15',
 'registered', '2024-08-15', '2024-10-22', 'EY123456', '2024-10-22',
 'low', 100, 'domestic', '42 Willow Gardens, Bristol, BS8 4TH', 'Bristol',
 '["0-5","5-7"]',
 '{"dbs":{"status":"complete","date":"2024-08-28","certificate":"001234567890","details":"Clear - No disclosures"},"dbs_update":{"status":"complete","date":"2024-08-28","subscriptionId":"U123456789"},"la_check":{"status":"complete","date":"2024-09-10","details":"No concerns - Bristol LA confirmed no records"},"ofsted":{"status":"complete","date":"2024-09-05","details":"Not previously known to Ofsted","knownToOfsted":false},"gp_health":{"status":"complete","date":"2024-09-15","details":"Fit to work with children - Dr. Smith, Clifton Medical Centre"},"ref_1":{"status":"complete","date":"2024-09-08","referee":"Jane Williams","relationship":"Former Employer (Bright Stars Nursery)","recommendation":"Excellent"},"ref_2":{"status":"complete","date":"2024-09-12","referee":"Dr. Michael Chen","relationship":"Professional (Health Visitor)","recommendation":"Highly Recommended"},"first_aid":{"status":"complete","date":"2024-08-20","provider":"St Johns Ambulance","expiryDate":"2027-08-20"},"safeguarding":{"status":"complete","date":"2024-08-22","provider":"EduCare","level":"Level 3"},"food_hygiene":{"status":"complete","date":"2024-08-25","level":"Level 2"},"insurance":{"status":"complete","date":"2024-10-01","provider":"Morton Michel","policyNumber":"MM-2024-789012","expiryDate":"2025-10-01"}}',
 '[{"id":"CP-RM-001","name":"David Morrison","type":"household","relationship":"Spouse","dob":"1983-07-22","formStatus":"complete","formSubmittedDate":"2024-08-25","formType":"CMA-H2","address":"42 Willow Gardens, Bristol, BS8 4TH","moveInDate":"2015-06-01","previousNames":[],"checks":{"dbs":{"status":"complete","date":"2024-09-02","certificate":"001234567891","details":"Clear"},"la_check":{"status":"complete","date":"2024-09-12","details":"No concerns"}},"declaration":{"signed":true,"date":"2024-08-25"}},{"id":"CP-RM-002","name":"Sophie Morrison","type":"household","relationship":"Daughter (aged 17)","dob":"2007-04-10","formStatus":"complete","formSubmittedDate":"2024-08-26","formType":"CMA-H2","checks":{"dbs":{"status":"complete","date":"2024-09-05","certificate":"001234567892","details":"Clear"},"la_check":{"status":"complete","date":"2024-09-15","details":"No concerns"}},"declaration":{"signed":true,"date":"2024-08-26"}}]',
 '{"requestDate":"2024-09-01","responseDate":"2024-09-05","status":"Not Known","details":"Individual is not known to Ofsted. No previous registrations found.","checkedBy":"Ofsted Records Team","referenceNumber":"OS-2024-78901"}'
),

-- 2. Angela Patterson - REGISTERED with Assistant
('RK-2024-00105', 'Angela', 'Patterson', 'angela.p@email.com', '07891 234567', '1979-11-08',
 'registered', '2024-09-01', '2024-11-15', 'EY234567', '2024-11-15',
 'low', 100, 'domestic', '18 Oak Lane, Leeds, LS6 2AB', 'Leeds',
 '["0-5","5-7","8+"]',
 '{"dbs":{"status":"complete","date":"2024-09-15","certificate":"001234567893","details":"Clear"},"dbs_update":{"status":"complete","date":"2024-09-15"},"la_check":{"status":"complete","date":"2024-09-25","details":"No concerns - Leeds confirmed"},"ofsted":{"status":"complete","date":"2024-09-18","details":"Previously registered - EY098765 (2015-2019) - No concerns","knownToOfsted":true,"previousReg":"EY098765"},"gp_health":{"status":"complete","date":"2024-09-28","details":"Fit to work"},"ref_1":{"status":"complete","date":"2024-09-20","referee":"Sarah Holmes","relationship":"Previous Parent"},"ref_2":{"status":"complete","date":"2024-09-22","referee":"Mark Thompson","relationship":"Professional"},"first_aid":{"status":"complete","date":"2024-09-10","expiryDate":"2027-09-10"},"safeguarding":{"status":"complete","date":"2024-09-12"},"food_hygiene":{"status":"complete","date":"2024-09-14"},"insurance":{"status":"complete","date":"2024-11-01","provider":"Pacey Insurance","policyNumber":"PI-2024-345678"}}',
 '[{"id":"CP-AP-001","name":"Karen Mitchell","type":"assistant","relationship":"Assistant (Part-time)","dob":"1990-05-20","email":"karen.m@email.com","formStatus":"complete","formSubmittedDate":"2024-09-18","formType":"CMA-A1","qualifications":["Level 2 Childcare","Paediatric First Aid"],"employmentHistory":[{"employer":"Little Stars Nursery","role":"Nursery Assistant","from":"2018-03","to":"2024-08"},{"employer":"Self-employed Nanny","role":"Nanny","from":"2015-06","to":"2018-02"}],"checks":{"dbs":{"status":"complete","date":"2024-09-28","certificate":"001234567894","details":"Clear"},"la_check":{"status":"complete","date":"2024-10-05","details":"No concerns"},"ref_1":{"status":"complete","date":"2024-10-02","referee":"Little Stars Nursery"}},"declaration":{"signed":true,"date":"2024-09-18"}},{"id":"CP-AP-002","name":"Robert Patterson","type":"household","relationship":"Husband","dob":"1977-02-14","formStatus":"complete","formSubmittedDate":"2024-09-12","formType":"CMA-H2","checks":{"dbs":{"status":"complete","date":"2024-09-22","certificate":"001234567895","details":"Clear"},"la_check":{"status":"complete","date":"2024-09-28","details":"No concerns"}}}]',
 '{"requestDate":"2024-09-12","responseDate":"2024-09-18","status":"Previously Registered","details":"Previously registered as childminder (EY098765) from 2015-2019. Registration voluntarily cancelled. No enforcement actions. Last inspection: Good (2018).","previousRegistrations":[{"number":"EY098765","type":"Childminder","from":"2015-04-01","to":"2019-08-31","reason":"Voluntary cancellation","lastInspection":"Good"}],"checkedBy":"Ofsted Records Team","referenceNumber":"OS-2024-78902"}'
),

-- 3. Fatima Al-Hassan - REGISTERED with multiple assistants
('RK-2024-00112', 'Fatima', 'Al-Hassan', 'fatima.ah@email.com', '07456 789012', '1982-09-25',
 'registered', '2024-09-20', '2024-12-05', 'EY345678', '2024-12-05',
 'low', 100, 'domestic', '7 Crescent Road, Manchester, M20 4BT', 'Manchester',
 '["0-5"]',
 '{"dbs":{"status":"complete","date":"2024-10-02","certificate":"001234567896","details":"Clear"},"dbs_update":{"status":"complete","date":"2024-10-02"},"la_check":{"status":"complete","date":"2024-10-15","details":"No concerns - Manchester"},"ofsted":{"status":"complete","date":"2024-10-08","details":"Not known to Ofsted","knownToOfsted":false},"gp_health":{"status":"complete","date":"2024-10-20","details":"Fit to work"},"ref_1":{"status":"complete","date":"2024-10-10"},"ref_2":{"status":"complete","date":"2024-10-12"},"first_aid":{"status":"complete","date":"2024-09-28","expiryDate":"2027-09-28"},"safeguarding":{"status":"complete","date":"2024-09-30"},"food_hygiene":{"status":"complete","date":"2024-10-01"},"insurance":{"status":"complete","date":"2024-11-20"}}',
 '[{"id":"CP-FA-001","name":"Yusuf Al-Hassan","type":"household","relationship":"Husband","dob":"1980-03-12","formStatus":"complete","formType":"CMA-H2","checks":{"dbs":{"status":"complete","date":"2024-10-12","details":"Clear"},"la_check":{"status":"complete","date":"2024-10-20","details":"No concerns"}}},{"id":"CP-FA-002","name":"Amira Al-Hassan","type":"assistant","relationship":"Assistant (Full-time)","dob":"1995-07-18","formStatus":"complete","formType":"CMA-A1","qualifications":["Level 3 Early Years","Paediatric First Aid","Safeguarding Level 2"],"checks":{"dbs":{"status":"complete","date":"2024-10-18","details":"Clear"},"la_check":{"status":"complete","date":"2024-10-25","details":"No concerns"},"ref_1":{"status":"complete","date":"2024-10-22"}}},{"id":"CP-FA-003","name":"Layla Mahmoud","type":"assistant","relationship":"Assistant (Part-time)","dob":"1992-11-05","formStatus":"complete","formType":"CMA-A1","checks":{"dbs":{"status":"complete","date":"2024-10-22","details":"Clear"},"la_check":{"status":"complete","date":"2024-10-28","details":"No concerns"},"ref_1":{"status":"complete","date":"2024-10-25"}}}]',
 '{"requestDate":"2024-10-01","responseDate":"2024-10-08","status":"Not Known","details":"No previous registrations found. Individual not known to Ofsted.","checkedBy":"Ofsted Records Team","referenceNumber":"OS-2024-78903"}'
),

-- 4. Emma Thompson - APPROVED
('RK-2024-00125', 'Emma', 'Thompson', 'emma.t@email.com', '07234 567890', '1988-06-12',
 'approved', '2024-10-15', NULL, NULL, '2024-12-16',
 'low', 100, 'domestic', '23 Maple Street, Sheffield, S10 2GR', 'Sheffield',
 '["0-5","5-7"]',
 '{"dbs":{"status":"complete","date":"2024-10-28","certificate":"001234567897","details":"Clear"},"dbs_update":{"status":"complete","date":"2024-10-28"},"la_check":{"status":"complete","date":"2024-11-08","details":"No concerns"},"ofsted":{"status":"complete","date":"2024-11-02","details":"Not known","knownToOfsted":false},"gp_health":{"status":"complete","date":"2024-11-12","details":"Fit to work"},"ref_1":{"status":"complete","date":"2024-11-05"},"ref_2":{"status":"complete","date":"2024-11-08"},"first_aid":{"status":"complete","date":"2024-10-20"},"safeguarding":{"status":"complete","date":"2024-10-22"},"food_hygiene":{"status":"complete","date":"2024-10-25"},"insurance":{"status":"complete","date":"2024-12-10"}}',
 '[]',
 '{"requestDate":"2024-10-28","responseDate":"2024-11-02","status":"Not Known","details":"Individual not known to Ofsted.","referenceNumber":"OS-2024-78904"}'
),

-- 5. Sarah Johnson - REVIEW
('RK-2024-00130', 'Sarah', 'Johnson', 'sarah.johnson@email.com', '07345 678901', '1990-02-28',
 'review', '2024-11-01', NULL, NULL, '2024-12-17',
 'low', 95, 'domestic', '15 Rose Avenue, Bristol, BS3 4LP', 'Bristol',
 '["0-5","5-7"]',
 '{"dbs":{"status":"complete","date":"2024-11-15","details":"Clear"},"dbs_update":{"status":"complete","date":"2024-11-15"},"la_check":{"status":"complete","date":"2024-11-25","details":"No concerns"},"ofsted":{"status":"complete","date":"2024-11-18","details":"Not known","knownToOfsted":false},"gp_health":{"status":"complete","date":"2024-12-01"},"ref_1":{"status":"complete","date":"2024-11-20"},"ref_2":{"status":"complete","date":"2024-11-22"},"first_aid":{"status":"complete","date":"2024-10-28"},"safeguarding":{"status":"complete","date":"2024-10-30"},"food_hygiene":{"status":"complete","date":"2024-11-02"},"insurance":{"status":"pending","date":null,"details":"Awaiting certificate upload"}}',
 '[{"id":"CP-SJ-001","name":"Michael Johnson","type":"household","relationship":"Spouse","dob":"1988-08-15","formStatus":"complete","formType":"CMA-H2","checks":{"dbs":{"status":"complete","date":"2024-11-22","details":"Clear"},"la_check":{"status":"complete","date":"2024-11-30","details":"No concerns"}}}]',
 '{"requestDate":"2024-11-12","responseDate":"2024-11-18","status":"Not Known","referenceNumber":"OS-2024-78905"}'
),

-- 6. Andrew Clarke - CHECKS
('RK-2024-00135', 'Andrew', 'Clarke', 'andrew.c@email.com', '07456 789012', '1985-04-20',
 'checks', '2024-11-10', NULL, NULL, '2024-12-17',
 'medium', 70, 'domestic', '8 Elm Court, Newcastle, NE3 4RT', 'Newcastle',
 '["0-5","5-7","8+"]',
 '{"dbs":{"status":"complete","date":"2024-11-25","details":"Clear"},"dbs_update":{"status":"complete","date":"2024-11-25"},"la_check":{"status":"complete","date":"2024-12-05","details":"No concerns"},"ofsted":{"status":"complete","date":"2024-11-28","details":"Not known"},"gp_health":{"status":"pending","date":"2024-12-10","details":"Awaiting GP response - follow-up sent"},"ref_1":{"status":"complete","date":"2024-12-01"},"ref_2":{"status":"complete","date":"2024-12-03"},"first_aid":{"status":"complete","date":"2024-11-15"},"safeguarding":{"status":"complete","date":"2024-11-18"},"food_hygiene":{"status":"complete","date":"2024-11-20"},"insurance":{"status":"pending","date":null}}',
 '[{"id":"CP-AC-001","name":"Mary Clarke","type":"assistant","relationship":"Assistant","dob":"1992-10-08","formStatus":"complete","formType":"CMA-A1","checks":{"dbs":{"status":"complete","date":"2024-12-05","details":"Clear"},"la_check":{"status":"complete","date":"2024-12-12","details":"No concerns"},"ref_1":{"status":"complete","date":"2024-12-08"}}}]',
 NULL
),

-- 7. Priya Patel - CHECKS
('RK-2024-00138', 'Priya', 'Patel', 'priya.p@email.com', '07567 890123', '1991-12-05',
 'checks', '2024-11-20', NULL, NULL, '2024-12-16',
 'low', 55, 'domestic', '22 Orchard Way, Leicester, LE2 3TY', 'Leicester',
 '["0-5"]',
 '{"dbs":{"status":"pending","date":"2024-12-01","details":"Application submitted - awaiting certificate"},"dbs_update":{"status":"not-started","date":null},"la_check":{"status":"pending","date":"2024-12-05","details":"Request sent"},"ofsted":{"status":"pending","date":"2024-12-02","details":"Awaiting response"},"gp_health":{"status":"not-started","date":null},"ref_1":{"status":"pending","date":"2024-12-03","details":"Request sent"},"ref_2":{"status":"pending","date":"2024-12-03","details":"Request sent"},"first_aid":{"status":"complete","date":"2024-11-15"},"safeguarding":{"status":"pending","date":null,"details":"Booked for 2024-12-20"},"food_hygiene":{"status":"not-started","date":null},"insurance":{"status":"not-started","date":null}}',
 '[{"id":"CP-PP-001","name":"Raj Patel","type":"household","relationship":"Husband","formStatus":"complete","formType":"CMA-H2","checks":{"dbs":{"status":"pending","date":"2024-12-05","details":"Submitted"},"la_check":{"status":"not-started","date":null}}},{"id":"CP-PP-002","name":"Meera Patel","type":"household","relationship":"Mother-in-law","formStatus":"pending","formType":"CMA-H2","checks":{"dbs":{"status":"not-started","date":null},"la_check":{"status":"not-started","date":null}}}]',
 NULL
),

-- 8. David Williams - BLOCKED
('RK-2024-00128', 'David', 'Williams', 'david.w@email.com', '07678 901234', '1983-08-30',
 'blocked', '2024-10-28', NULL, NULL, '2024-12-14',
 'high', 45, 'domestic', '5 Station Road, Manchester, M15 4PR', 'Manchester',
 '["0-5","5-7","8+"]',
 '{"dbs":{"status":"blocked","date":"2024-12-14","certificate":"001234567898","details":"Disclosure: Caution for minor traffic offence (2019). Risk assessment required."},"dbs_update":{"status":"pending","date":null},"la_check":{"status":"pending","date":"2024-12-01","details":"Awaiting response"},"ofsted":{"status":"complete","date":"2024-11-20","details":"Previously registered - no concerns","knownToOfsted":true},"gp_health":{"status":"not-started","date":null},"ref_1":{"status":"complete","date":"2024-11-25"},"ref_2":{"status":"complete","date":"2024-11-28"},"first_aid":{"status":"not-started","date":null},"safeguarding":{"status":"not-started","date":null},"food_hygiene":{"status":"not-started","date":null},"insurance":{"status":"not-started","date":null}}',
 '[{"id":"CP-DW-001","name":"Lisa Williams","type":"assistant","relationship":"Assistant","formStatus":"pending","formType":"CMA-A1","checks":{"dbs":{"status":"not-started","date":null},"la_check":{"status":"not-started","date":null}}}]',
 '{"requestDate":"2024-11-15","responseDate":"2024-11-20","status":"Previously Registered","details":"Previously registered (EY555666) 2018-2021. No enforcement actions.","referenceNumber":"OS-2024-78906"}'
),

-- 9. Jennifer Brown - FORM-SUBMITTED
('RK-2024-00142', 'Jennifer', 'Brown', 'jennifer.b@email.com', '07789 012345', '1987-05-18',
 'form-submitted', '2024-12-10', NULL, NULL, '2024-12-15',
 'low', 20, 'domestic', '12 Cherry Lane, Birmingham, B15 2TR', 'Birmingham',
 '["0-5","5-7"]',
 '{"dbs":{"status":"pending","date":"2024-12-15","details":"Application initiated"},"dbs_update":{"status":"not-started","date":null},"la_check":{"status":"not-started","date":null},"ofsted":{"status":"pending","date":"2024-12-15","details":"Request submitted"},"gp_health":{"status":"not-started","date":null},"ref_1":{"status":"pending","date":"2024-12-15","details":"Request sent"},"ref_2":{"status":"pending","date":"2024-12-15","details":"Request sent"},"first_aid":{"status":"complete","date":"2024-12-01"},"safeguarding":{"status":"complete","date":"2024-12-05"},"food_hygiene":{"status":"not-started","date":null},"insurance":{"status":"not-started","date":null}}',
 '[]',
 NULL
),

-- 10. James Mitchell - NEW
('RK-2024-00145', 'James', 'Mitchell', 'james.m@email.com', '07890 123456', '1984-01-25',
 'new', '2024-12-15', NULL, NULL, '2024-12-15',
 'low', 5, 'non-domestic', 'Unit 5, Community Centre, Birmingham, B20 3RD', 'Birmingham',
 '["0-5","5-7"]',
 '{"dbs":{"status":"not-started","date":null},"dbs_update":{"status":"not-started","date":null},"la_check":{"status":"not-started","date":null},"ofsted":{"status":"not-started","date":null},"gp_health":{"status":"not-started","date":null},"ref_1":{"status":"not-started","date":null},"ref_2":{"status":"not-started","date":null},"first_aid":{"status":"not-started","date":null},"safeguarding":{"status":"not-started","date":null},"food_hygiene":{"status":"not-started","date":null},"insurance":{"status":"not-started","date":null}}',
 '[{"id":"CP-JM-001","name":"Sophie Mitchell","type":"assistant","relationship":"Assistant","formStatus":"not-started","formType":"CMA-A1","checks":{"dbs":{"status":"not-started","date":null},"la_check":{"status":"not-started","date":null}}}]',
 NULL
),

-- 11. Lucy Chen - NEW
('RK-2024-00146', 'Lucy', 'Chen', 'lucy.c@email.com', '07901 234567', '1993-07-10',
 'new', '2024-12-17', NULL, NULL, '2024-12-17',
 'low', 5, 'domestic', NULL, 'London - Camden',
 '["0-5"]',
 '{"dbs":{"status":"not-started","date":null},"dbs_update":{"status":"not-started","date":null},"la_check":{"status":"not-started","date":null},"ofsted":{"status":"not-started","date":null},"gp_health":{"status":"not-started","date":null},"ref_1":{"status":"not-started","date":null},"ref_2":{"status":"not-started","date":null},"first_aid":{"status":"not-started","date":null},"safeguarding":{"status":"not-started","date":null},"food_hygiene":{"status":"not-started","date":null},"insurance":{"status":"not-started","date":null}}',
 '[]',
 NULL
),

-- 12. Helen Roberts - CHECKS
('RK-2024-00133', 'Helen', 'Roberts', 'helen.r@email.com', '07012 345678', '1986-11-15',
 'checks', '2024-11-05', NULL, NULL, '2024-12-16',
 'medium', 65, 'domestic', '34 Park View, Liverpool, L18 1DF', 'Liverpool',
 '["0-5","5-7"]',
 '{"dbs":{"status":"complete","date":"2024-11-20","details":"Clear"},"dbs_update":{"status":"complete","date":"2024-11-20"},"la_check":{"status":"pending","date":"2024-11-25","details":"Follow-up sent to Liverpool LA"},"ofsted":{"status":"complete","date":"2024-11-22","details":"Not known"},"gp_health":{"status":"complete","date":"2024-12-05","details":"Fit to work"},"ref_1":{"status":"complete","date":"2024-11-28"},"ref_2":{"status":"pending","date":"2024-11-28","details":"Awaiting response from referee"},"first_aid":{"status":"complete","date":"2024-11-01"},"safeguarding":{"status":"complete","date":"2024-11-03"},"food_hygiene":{"status":"not-started","date":null},"insurance":{"status":"not-started","date":null}}',
 '[{"id":"CP-HR-001","name":"Peter Roberts","type":"household","relationship":"Partner","formStatus":"complete","formType":"CMA-H2","checks":{"dbs":{"status":"complete","date":"2024-12-01","details":"Clear"},"la_check":{"status":"pending","date":"2024-12-05"}}},{"id":"CP-HR-002","name":"Tom Roberts","type":"household","relationship":"Son (aged 18)","formStatus":"complete","formType":"CMA-H2","checks":{"dbs":{"status":"complete","date":"2024-12-03","details":"Clear"},"la_check":{"status":"pending","date":"2024-12-08"}}}]',
 NULL
)
ON CONFLICT (id) DO NOTHING;

-- Timeline events for all seed applications
INSERT INTO timeline_events (application_id, event, type, created_at) VALUES
-- Rebecca Morrison
('RK-2024-00098', 'Application started', 'action', '2024-08-15 09:00:00+00'),
('RK-2024-00098', 'Application form submitted', 'complete', '2024-08-20 14:00:00+00'),
('RK-2024-00098', 'DBS certificate received - clear', 'complete', '2024-08-28 10:15:00+00'),
('RK-2024-00098', 'All household DBS checks complete', 'complete', '2024-09-02 16:00:00+00'),
('RK-2024-00098', 'Ofsted history check - not known', 'complete', '2024-09-05 11:00:00+00'),
('RK-2024-00098', 'Reference 1 received - excellent', 'complete', '2024-09-08 09:30:00+00'),
('RK-2024-00098', 'LA check completed - no concerns', 'complete', '2024-09-10 14:00:00+00'),
('RK-2024-00098', 'Reference 2 received - positive', 'complete', '2024-09-12 10:00:00+00'),
('RK-2024-00098', 'GP health declaration received - fit to work', 'complete', '2024-09-15 15:30:00+00'),
('RK-2024-00098', 'Insurance certificate received', 'complete', '2024-10-01 11:00:00+00'),
('RK-2024-00098', 'Final review completed - APPROVED', 'complete', '2024-10-18 09:00:00+00'),
('RK-2024-00098', 'Ofsted notification submitted', 'complete', '2024-10-20 14:30:00+00'),
('RK-2024-00098', 'Registration certificate issued - EY123456', 'complete', '2024-10-22 10:00:00+00'),

-- Angela Patterson
('RK-2024-00105', 'Assistant Karen Mitchell fully cleared', 'complete', '2024-10-05 14:00:00+00'),
('RK-2024-00105', 'Final review completed - APPROVED', 'complete', '2024-11-12 09:00:00+00'),
('RK-2024-00105', 'Registration certificate issued - EY234567', 'complete', '2024-11-15 11:00:00+00'),

-- Fatima Al-Hassan
('RK-2024-00112', 'Final review - APPROVED', 'complete', '2024-12-02 14:00:00+00'),
('RK-2024-00112', 'Registration certificate issued - EY345678', 'complete', '2024-12-05 10:00:00+00'),

-- Emma Thompson
('RK-2024-00125', 'Insurance certificate received', 'complete', '2024-12-10 14:00:00+00'),
('RK-2024-00125', 'Application APPROVED - preparing Ofsted notification', 'complete', '2024-12-16 09:00:00+00'),

-- Sarah Johnson
('RK-2024-00130', 'Insurance certificate requested', 'action', '2024-12-15 14:00:00+00'),
('RK-2024-00130', 'Moved to final review - awaiting insurance', 'action', '2024-12-17 10:00:00+00'),

-- Andrew Clarke
('RK-2024-00135', 'GP health declaration form sent', 'action', '2024-12-10 14:00:00+00'),
('RK-2024-00135', 'Second follow-up sent to GP surgery', 'action', '2024-12-17 09:00:00+00'),

-- Priya Patel
('RK-2024-00138', 'Reminder sent to Meera Patel for CMA-H2 form', 'action', '2024-12-16 11:00:00+00'),

-- David Williams
('RK-2024-00128', 'DBS returned with disclosure - ESCALATED for risk assessment', 'alert', '2024-12-14 11:00:00+00'),
('RK-2024-00128', 'Application BLOCKED pending suitability review', 'alert', '2024-12-14 11:30:00+00'),

-- Jennifer Brown
('RK-2024-00142', 'Application form submitted', 'complete', '2024-12-12 14:00:00+00'),
('RK-2024-00142', 'Application form reviewed and accepted', 'complete', '2024-12-15 09:30:00+00'),
('RK-2024-00142', 'DBS application initiated', 'action', '2024-12-15 10:00:00+00'),

-- James Mitchell
('RK-2024-00145', 'Application invitation sent', 'action', '2024-12-15 16:00:00+00'),

-- Lucy Chen
('RK-2024-00146', 'Application invitation sent', 'action', '2024-12-17 11:00:00+00'),

-- Helen Roberts
('RK-2024-00133', 'Follow-up sent to Liverpool LA', 'action', '2024-12-10 14:00:00+00'),
('RK-2024-00133', 'Follow-up sent to Reference 2', 'action', '2024-12-16 09:00:00+00')
ON CONFLICT DO NOTHING;
