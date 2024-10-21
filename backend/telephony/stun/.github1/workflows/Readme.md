# Instagram Architecture

## Bottlenecks: 
1. Saving images/videos or other files takes a lot of time.
2. Syncronisation process - it prepares data for ready-to-read format. Need some time to finish this process.

## SPOF:
1. Gateway service:
        - CPU
2. Load balancer:
        - I/O operations
        - CPU
3. Post service: 
        - I/O operation
        - CPU 
4. Media Service:
        - I/O operations
        - CPU
        - RAM
6. Batch Event Service:
        - I/O operations
        - CPU
        - RAM
7. Sync Service:
    - I/O operations
    - CPU
    - RAM
