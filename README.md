# Read_m3u_file

A simple ASP.NET Core MVC application that parses `.m3u` and `.m3u8` playlist files and plays videos using an HTML5 video player or HLS.js for HLS streams.

---

## Features

- Parse `.m3u` or `.m3u8` playlist files to extract media URLs.
- Play video files using the HTML5 `<video>` element.
- Support for HLS (HTTP Live Streaming) using [HLS.js](https://github.com/video-dev/hls.js).

---

## Requirements

- **.NET 8 SDK**
- A `.m3u` or `.m3u8` file with video URLs or HLS stream links.
- Modern web browser (for HLS, browsers that support JavaScript and HLS.js).

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/gamalmouhssine/read_m3u_file.git
cd read_m3u_file


