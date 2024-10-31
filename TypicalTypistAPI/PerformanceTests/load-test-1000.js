import http from 'k6/http';
import { sleep } from 'k6';
import { textSummary } from 'https://jslib.k6.io/k6-summary/0.0.2/index.js';

export const options = {
    stages: [
        { duration: '30s', target: 1000 },
        { duration: '1m', target: 1000 },
        { duration: '30s', target: 0 },
    ],
};

export default function () {
    const url = 'https://localhost:7258/api/Words/Random';
    let res = http.get(url);

    if (res.status !== 200) {
        console.error(`Request failed with status: ${res.status}`);
    }

    sleep(5);
}

// Use handleSummary for basic view... comment out for full standard view
export function handleSummary(data) {
    delete data.metrics['http_req_duration{expected_response:true}'];
    delete data.metrics['http_req_waiting'];
    delete data.metrics['http_req_blocked'];
    delete data.metrics['http_req_connecting'];
    delete data.metrics['http_req_receiving'];
    delete data.metrics['http_req_sending'];
    delete data.metrics['http_req_tls_handshaking'];

    for (const key in data.metrics) {
        if (key.startsWith('iteration')) delete data.metrics[key];
    }

    for (const key in data.metrics) {
        if (key.startsWith('vu')) delete data.metrics[key];
    }

    for (const key in data.metrics) {
        if (key.startsWith('data')) delete data.metrics[key];
    }

    return {
        stdout: textSummary(data, { indent: '→', enableColors: true }),
    };
}
