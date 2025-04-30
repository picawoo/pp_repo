// Инициализация карты
const map = L.map('map').setView([56.8431, 60.6454], 2);
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap contributors'
}).addTo(map);

document.addEventListener('DOMContentLoaded', function() {
    // График упоминаний
    const mentionsCtx = document.getElementById('mentionsChart').getContext('2d');
    new Chart(mentionsCtx, {
        type: 'line',
        data: {
            labels: ['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн'],
            datasets: [{
                label: 'Количество упоминаний',
                data: [65, 59, 80, 81, 56, 55],
                borderColor: '#003366',
                tension: 0.1,
                fill: true,
                backgroundColor: 'rgba(0, 51, 102, 0.1)'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)'
                    }
                },
                x: {
                    grid: {
                        display: false
                    }
                }
            }
        }
    });

    // Гистограмма факультетов
    const facultiesCtx = document.getElementById('facultiesChart').getContext('2d');
    new Chart(facultiesCtx, {
        type: 'bar',
        data: {
            labels: ['ИРИТ-РТФ', 'ИЕНиМ', 'ИНМТ', 'ИСО', 'ИСПН'],
            datasets: [{
                data: [30, 25, 20, 15, 10],
                backgroundColor: [
                    '#003366',
                    '#FF6B00',
                    '#666666',
                    '#4CAF50',
                    '#FFC107'
                ]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)'
                    }
                },
                x: {
                    grid: {
                        display: false
                    }
                }
            }
        }
    });

    // График сфер деятельности
    const sectorsCtx = document.getElementById('sectorsChart').getContext('2d');
    new Chart(sectorsCtx, {
        type: 'pie',
        data: {
            labels: ['Бизнес', 'Наука', 'Госсектор', 'Образование', 'Другое'],
            datasets: [{
                data: [40, 25, 20, 10, 5],
                backgroundColor: [
                    '#003366',
                    '#FF6B00',
                    '#666666',
                    '#4CAF50',
                    '#FFC107'
                ]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right'
                }
            }
        }
    });
});


// Добавление маркеров на карту
function addMarkers(data) {
    data.forEach(item => {
        const marker = L.marker([item.lat, item.lng])
            .bindPopup(`
                <strong>${item.name}</strong><br>
                Упоминаний: ${item.mentions}<br>
                Последнее: ${item.lastMention}
            `);
        
        marker.addTo(map);
    });
}

// Пример данных для маркеров
const sampleData = [
    { name: 'Москва', lat: 55.7558, lng: 37.6173, mentions: 45, lastMention: '12.03.2024' },
    { name: 'Санкт-Петербург', lat: 59.9343, lng: 30.3351, mentions: 30, lastMention: '11.03.2024' },
    { name: 'Екатеринбург', lat: 56.8431, lng: 60.6454, mentions: 25, lastMention: '10.03.2024' }
];

addMarkers(sampleData); 