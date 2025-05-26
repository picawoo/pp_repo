from crawler import NewsCrawler
from analyzer import ContentAnalyzer
from database import MentionDatabase
import requests
import time
import json

class MonitoringService:
    def __init__(self):
        self.crawler = NewsCrawler()
        self.analyzer = ContentAnalyzer()
        self.db = MentionDatabase()
        self.config = self.load_config()

    def load_config(self):
        try:
            with open('config.json') as f:
                return json.load(f)
        except:
            return {
                'check_interval': 3600,
                'max_articles_per_run': 100
            }

    def fetch_article(self, url):
        try:
            response = requests.get(
                url,
                headers={'User-Agent': 'Mozilla/5.0'},
                timeout=15
            )
            response.raise_for_status()
            return response.text
        except Exception as e:
            print(f"Failed to fetch {url}: {str(e)}")
            return None

    def process_article(self, article_info):
        html = self.fetch_article(article_info['url'])
        if html:
            analysis_result = self.analyzer.analyze_article(
                article_info['url'],
                html
            )
            if analysis_result['persons']:
                article_info.update(analysis_result)
                self.db.save_mention(article_info)
                print(f"Found mention in: {article_info['url']}")

    def run(self):
        while True:
            try:
                print("Starting new monitoring cycle...")
                news_links = self.crawler.get_fresh_news_links(hours=24)
                print(f"Found {len(news_links)} new articles")
                
                for article in news_links[:self.config['max_articles_per_run']]:
                    if article['url'] not in self.db.get_processed_urls():
                        self.process_article(article)
                
                print(f"Sleeping for {self.config['check_interval']} seconds...")
                time.sleep(self.config['check_interval'])
                
            except KeyboardInterrupt:
                print("Stopping monitor...")
                break
            except Exception as e:
                print(f"Critical error: {str(e)}")
                time.sleep(60)

if __name__ == "__main__":
    monitor = MonitoringService()
    monitor.run()