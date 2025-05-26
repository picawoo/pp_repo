import requests
from bs4 import BeautifulSoup
from urllib.parse import urljoin
from datetime import datetime, timedelta
import re

class NewsCrawler:
    def __init__(self):
        self.visited_urls = set()
        self.news_sites = [
            {
                'name': 'РБК',
                'url': 'https://www.rbc.ru',
                'news_selector': 'a.news-feed__item',
                'base_url': 'https://www.rbc.ru',
                'date_selector': 'div.article__header__date',
                'date_format': '%H:%M %d.%m.%Y'
            },
            {
                'name': 'Коммерсантъ',
                'url': 'https://www.kommersant.ru',
                'news_selector': 'a.article-card__link',
                'base_url': 'https://www.kommersant.ru',
                'date_selector': 'time.article_header__time',
                'date_format': '%d.%m.%Y %H:%M'
            },
            {
                'name': 'E1.ru',
                'url': 'https://www.e1.ru/news/',
                'news_selector': 'a.news-item__title',
                'base_url': 'https://www.e1.ru',
                'date_selector': 'span.news-item__date-text',
                'date_format': '%H:%M %d.%m.%Y',
                'is_regional': True
            },
            {
                'name': 'URA.news',
                'url': 'https://ura.news',
                'news_selector': 'a.news-card__title',
                'base_url': 'https://ura.news',
                'date_selector': 'div.news-card__date',
                'date_format': '%H:%M %d.%m.%Y',
                'is_regional': True
            },
            {
                'name': '66.ru',
                'url': 'https://66.ru',
                'news_selector': 'a.news-feed__item',
                'base_url': 'https://66.ru',
                'date_selector': 'div.news-feed__item-date',
                'date_format': '%H:%M %d.%m.%Y',
                'is_regional': True
            },
            {
                'name': 'KP Урал',
                'url': 'https://www.ural.kp.ru',
                'news_selector': 'a.css-1f4x1hn',
                'base_url': 'https://www.ural.kp.ru',
                'date_selector': 'span.css-13apl4v',
                'date_format': '%H:%M %d.%m.%Y',
                'is_regional': True
            }
        ]
        self.regional_keywords = [
            "Екатеринбург", "Свердловск", "Урал", 
            "Свердловской области", "Уральский"
        ]

    def is_regional_news(self, text):
        return any(keyword.lower() in text.lower() 
                 for keyword in self.regional_keywords)

    def get_article_date(self, article_url, site_config):
        try:
            response = requests.get(article_url, timeout=10)
            soup = BeautifulSoup(response.text, 'lxml')
            
            if site_config.get('is_regional'):
                list_response = requests.get(site_config['url'], timeout=5)
                list_soup = BeautifulSoup(list_response.text, 'lxml')
                for item in list_soup.select(site_config['news_selector']):
                    if article_url in item['href']:
                        date_str = item.find_parent().select_one(
                            site_config['date_selector']).text.strip()
                        return datetime.strptime(date_str, site_config['date_format'])
            
            date_str = soup.select_one(site_config['date_selector']).text.strip()
            return datetime.strptime(date_str, site_config['date_format'])
        except Exception as e:
            print(f"Date parsing error for {article_url}: {str(e)}")
            return datetime.now()

    def get_fresh_news_links(self, hours=24):
        all_links = []
        cutoff_time = datetime.now() - timedelta(hours=hours)
        
        for site in self.news_sites:
            try:
                print(f"Crawling {site['name']}...")
                response = requests.get(
                    site['url'],
                    headers={'User-Agent': 'Mozilla/5.0'},
                    timeout=15
                )
                soup = BeautifulSoup(response.text, 'lxml')
                
                for link in soup.select(site['news_selector']):
                    try:
                        href = link.get('href', '')
                        if not href.startswith('http'):
                            href = urljoin(site['base_url'], href)
                        
                        if href not in self.visited_urls:
                            pub_date = self.get_article_date(href, site)
                            
                            if site.get('is_regional'):
                                title = link.text.strip()
                                if not self.is_regional_news(title):
                                    continue
                            
                            if pub_date >= cutoff_time:
                                all_links.append({
                                    'url': href,
                                    'source': site['name'],
                                    'date': pub_date,
                                    'is_regional': site.get('is_regional', False)
                                })
                                self.visited_urls.add(href)
                    except Exception as e:
                        print(f"Error processing link: {str(e)}")
                        
            except Exception as e:
                print(f"Error crawling {site['name']}: {str(e)}")
        
        return sorted(all_links, key=lambda x: x['date'], reverse=True)