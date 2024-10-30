import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
    stages: [
        { duration: '30s', target: 1000 }, // ramp up to target users over 30 seconds
        { duration: '1m', target: 1000 },  // maintain target users for 1 minute
        { duration: '30s', target: 0 },   // ramp down to 0 users over 30 seconds
    ],
};

export default function () {
    const url = 'https://localhost:7258/api/Words/Random'; 
    let res = http.get(url);

    // Optional: Measure response time and check status code
    if (res.status !== 200) {
        console.error(`Request failed with status: ${res.status}`);
    }

    // Sleep here essentially means that users will wait 1 sec between their http calls
    sleep(1); 
}