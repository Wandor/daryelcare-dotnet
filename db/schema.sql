-- ReadyKids CMA Database Schema

CREATE SEQUENCE IF NOT EXISTS application_id_seq START WITH 200;

CREATE TABLE IF NOT EXISTS applications (
    id                  TEXT PRIMARY KEY,
    title               TEXT,
    first_name          TEXT NOT NULL,
    middle_names        TEXT,
    last_name           TEXT NOT NULL,
    name                TEXT GENERATED ALWAYS AS (first_name || ' ' || last_name) STORED,
    email               TEXT NOT NULL,
    phone               TEXT,
    dob                 DATE,
    gender              TEXT,
    right_to_work       TEXT,
    ni_number           TEXT,

    home_address        JSONB,
    premises_type       TEXT,
    premises_address    TEXT,
    premises_details    JSONB,
    local_authority     TEXT,

    registers           JSONB DEFAULT '[]',
    service             JSONB,
    stage               TEXT DEFAULT 'new'
                        CHECK (stage IN ('new','form-submitted','checks','review','approved','blocked','registered')),
    risk                TEXT DEFAULT 'low',
    progress            INT DEFAULT 0,

    checks              JSONB DEFAULT '{}',
    connected_persons   JSONB DEFAULT '[]',
    ofsted_check        JSONB,

    previous_names      JSONB,
    address_history     JSONB,
    qualifications      JSONB,
    employment_history  JSONB,
    references_data     JSONB,
    household           JSONB,
    suitability         JSONB,
    declaration         JSONB,

    start_date          DATE,
    registration_date   DATE,
    registration_number TEXT,

    last_updated        TIMESTAMPTZ DEFAULT NOW(),
    created_at          TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS timeline_events (
    id              SERIAL PRIMARY KEY,
    application_id  TEXT NOT NULL REFERENCES applications(id) ON DELETE CASCADE,
    event           TEXT NOT NULL,
    type            TEXT DEFAULT 'action'
                    CHECK (type IN ('action','complete','alert','note')),
    created_at      TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_timeline_app_id ON timeline_events(application_id);
CREATE INDEX IF NOT EXISTS idx_applications_stage ON applications(stage);
