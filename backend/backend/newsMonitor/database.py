import sqlite3
from datetime import datetime

class MentionDatabase:
    def __init__(self, db_path='mentions.db'):
        self.conn = sqlite3.connect(db_path)
        self._init_db()

    def _init_db(self):
        self.conn.execute('''
        CREATE TABLE IF NOT EXISTS mentions (
            id INTEGER PRIMARY KEY,
            url TEXT UNIQUE,
            title TEXT,
            source TEXT,
            publish_date TEXT,
            added_date TEXT,
            processed BOOLEAN DEFAULT 0
        )
        ''')
        
        self.conn.execute('''
        CREATE TABLE IF NOT EXISTS alumni_mentions (
            id INTEGER PRIMARY KEY,
            mention_id INTEGER,
            name TEXT,
            context TEXT,
            FOREIGN KEY(mention_id) REFERENCES mentions(id)
        )
        ''')
        self.conn.commit()

    def save_mention(self, data):
        try:
            cursor = self.conn.cursor()
            cursor.execute('''
            INSERT OR IGNORE INTO mentions 
            (url, title, source, publish_date, added_date)
            VALUES (?, ?, ?, ?, ?)
            ''', (
                data['url'],
                data['title'],
                data['source'],
                data['date'].isoformat(),
                datetime.now().isoformat()
            ))
            
            mention_id = cursor.lastrowid
            for person in data.get('persons', []):
                cursor.execute('''
                INSERT INTO alumni_mentions 
                (mention_id, name, context)
                VALUES (?, ?, ?)
                ''', (mention_id, person['name'], person['context']))
            
            self.conn.commit()
            return True
        except Exception as e:
            print(f"Database error: {str(e)}")
            return False

    def get_processed_urls(self):
        cursor = self.conn.cursor()
        cursor.execute('SELECT url FROM mentions')
        return set(row[0] for row in cursor.fetchall())