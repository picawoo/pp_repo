document.addEventListener('DOMContentLoaded', function() {
    const API_URL = 'http://localhost:5296/api';
    
    async function loadTopAlumni() {
        try {
            const response = await fetch(`${API_URL}/alumni/top`);
            if (!response.ok) throw new Error('Ошибка API');
            const data = await response.json();
            
            const container = document.getElementById('topAlumniContainer');
            container.innerHTML = data.map((alumni, index) => `
                <a href="mentions.html?graduate=${alumni.id}" class="top-item">
                    <span class="rank">${index + 1}</span>
                    <span class="name">${alumni.fullName}</span>
                    <span class="count">${alumni.mentionsCount} упоминаний</span>
                </a>
            `).join('');
        } catch (error) {
            console.error('Ошибка загрузки выпускников:', error);
            document.getElementById('topAlumniContainer').innerHTML = 
                '<div class="error">Не удалось загрузить данные</div>';
        }
    }

    async function loadLatestMentions() {
        try {
            const response = await fetch(`${API_URL}/mentions/latest`);
            if (!response.ok) throw new Error('Ошибка API');
            const data = await response.json();
            
            const container = document.getElementById('latestMentionsContainer');
            container.innerHTML = data.map(mention => `
                <a href="${mention.sourceUrl}" class="news-item" target="_blank">
                    <div class="news-date">${new Date(mention.foundAt).toLocaleDateString()}</div>
                    <div class="news-title">${mention.title}</div>
                    <div class="news-source">${mention.source}</div>
                </a>
            `).join('');
        } catch (error) {
            console.error('Ошибка загрузки упоминаний:', error);
            document.getElementById('latestMentionsContainer').innerHTML = 
                '<div class="error">Не удалось загрузить упоминания</div>';
        }
    }

    async function loadFacultiesStats() {
        try {
            const response = await fetch(`${API_URL}/statistics/faculties`);
            if (!response.ok) throw new Error('Ошибка API');
            const data = await response.json();
            
            new Chart(
                document.getElementById('facultiesChart'),
                {
                    type: 'bar',
                    data: {
                        labels: data.labels,
                        datasets: [{
                            label: 'Упоминаний',
                            data: data.values,
                            backgroundColor: '#3a86ff'
                        }]
                    }
                }
            );
        } catch (error) {
            console.error('Ошибка загрузки статистики:', error);
        }
    }

    loadTopAlumni();
    loadLatestMentions();
    loadFacultiesStats();
});