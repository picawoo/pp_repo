import spacy
from datetime import datetime
import re

class ContentAnalyzer:
    def __init__(self):
        self.nlp = spacy.load("ru_core_news_lg")
        self.university_keywords = [
            "УрФУ", "Уральский федеральный", 
            "Уральский федеральный университет",
            "УПИ", "Уральский политехнический",
            "УрГУ", "Уральский государственный университет"
        ]
        self.alumni_patterns = [
            r"(выпускник[а-я]*\s+(?:УрФУ|Уральского федерального))",
            r"(закончил[а-я]*\s+(?:УрФУ|Уральский федеральный))",
            r"(студент[а-я]*\s+(?:УрФУ|Уральского федерального))"
        ]

    def is_alumni_mention(self, text):
        for pattern in self.alumni_patterns:
            if re.search(pattern, text, re.IGNORECASE):
                return True
        return False

    def extract_persons(self, text):
        doc = self.nlp(text)
        persons = []
        
        for ent in doc.ents:
            if ent.label_ == "PER":
                start = max(0, ent.start_char - 50)
                end = min(len(text), ent.end_char + 50)
                context = text[start:end]
                
                if self.is_alumni_mention(context):
                    persons.append({
                        'name': ent.text,
                        'context': context,
                        'position': (ent.start_char, ent.end_char)
                    })
        
        return persons

    def analyze_article(self, url, html):
        soup = BeautifulSoup(html, 'lxml')
        
        for elem in soup(['script', 'style', 'iframe', 'nav', 'footer']):
            elem.decompose()
        
        title = soup.title.get_text() if soup.title else 'Без названия'
        text = ' '.join(p.get_text() for p in soup.find_all(['p', 'div.article__text']))
        
        return {
            'url': url,
            'title': title,
            'text': text[:10000],
            'persons': self.extract_persons(text),
            'analysis_date': datetime.now().isoformat()
        }