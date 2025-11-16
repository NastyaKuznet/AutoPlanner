CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    nickname VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    telegram_chat_id BIGINT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tasks (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    priority INTEGER DEFAULT 0,
    start_date_time TIMESTAMP,
    end_date_time TIMESTAMP,
    duration INTERVAL,
    is_repit BOOLEAN DEFAULT FALSE,
    repit_time INTERVAL,
    is_repit_from_start BOOLEAN DEFAULT FALSE,
    count_repit INTEGER DEFAULT 0,
    start_date_time_repit TIMESTAMP,
    end_date_time_repit TIMESTAMP,
    rule_one_task BOOLEAN DEFAULT FALSE,
    start_date_time_rule_one_task TIMESTAMP,
    end_date_time_rule_one_task TIMESTAMP,
    rule_two_task BOOLEAN DEFAULT FALSE,
    time_position_regarding_task_id INTEGER DEFAULT 0,
    second_task_id INTEGER REFERENCES tasks(id) NULL,
    relation_range_id INTEGER DEFAULT 0,
    date_time_range INTERVAL,
    is_complete BOOLEAN DEFAULT FALSE,
    complete_date_time TIMESTAMP
);

CREATE TABLE IF NOT EXISTS timetable_items (
    my_task_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    count_from INTEGER NOT NULL DEFAULT 0,
    name VARCHAR(200) NOT NULL,
    priority INTEGER DEFAULT 0,
    start_date_time TIMESTAMP NOT NULL,
    end_date_time TIMESTAMP NOT NULL,
    is_complete BOOLEAN DEFAULT FALSE,
    complete_date_time TIMESTAMP,
    PRIMARY KEY (my_task_id, count_from)
);

CREATE TABLE IF NOT EXISTS planning_tasks (
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    my_task_id INTEGER NOT NULL,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    priority INTEGER DEFAULT 0,
    start_date_time TIMESTAMP,
    end_date_time TIMESTAMP,
    duration INTERVAL,
    count_from INTEGER NOT NULL DEFAULT 0,
    is_complete BOOLEAN DEFAULT FALSE,
    complete_date_time TIMESTAMP,
    start_date_time_range TIMESTAMP,
    end_date_time_range TIMESTAMP,
    rule_one_task BOOLEAN DEFAULT FALSE,
    start_date_time_rule_one_task TIMESTAMP,
    end_date_time_rule_one_task TIMESTAMP,
    rule_two_task BOOLEAN DEFAULT FALSE,
    time_position_regarding_task_id INTEGER DEFAULT 0,
    second_task_id INTEGER DEFAULT 0,
    relation_range_id INTEGER DEFAULT 0,
    date_time_range INTERVAL,
    PRIMARY KEY (user_id, my_task_id, count_from)
);