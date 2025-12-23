const http = require('http');
const https = require('https');
const fs = require('fs');
const path = require('path');
const url = require('url');

const PORT = process.env.PORT || 3001;
const ROOT = __dirname;

// Create HTTPS agent that ignores self-signed certificates
const httpsAgent = new https.Agent({
  rejectUnauthorized: false
});

const send = (res, status, body, headers = {}) => {
  res.writeHead(status, { 'Access-Control-Allow-Origin': '*', ...headers });
  res.end(body);
};

const serveStatic = (req, res) => {
  const parsed = url.parse(req.url);
  let filePath = parsed.pathname === '/' ? '/index.html' : parsed.pathname;
  const fullPath = path.join(ROOT, filePath);
  if (!fullPath.startsWith(ROOT)) return send(res, 403, 'Forbidden');
  fs.readFile(fullPath, (err, data) => {
    if (err) return send(res, 404, 'Not found');
    const ext = path.extname(fullPath).toLowerCase();
    const contentType = ext === '.html' ? 'text/html' : 'application/octet-stream';
    send(res, 200, data, { 'Content-Type': contentType });
  });
};

const proxy = async (req, res, target) => {
  try {
    const upstream = await fetch(target, { agent: httpsAgent });
    const buf = Buffer.from(await upstream.arrayBuffer());
    send(res, upstream.status, buf, {
      'Content-Type': upstream.headers.get('content-type') || 'application/octet-stream'
    });
  } catch (err) {
    console.error('Proxy error:', err);
    send(res, 500, 'proxy error');
  }
};

const server = http.createServer(async (req, res) => {
  const parsed = url.parse(req.url, true);
  if (req.method === 'OPTIONS') {
    return send(res, 200, '', {
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Methods': 'GET,OPTIONS',
      'Access-Control-Allow-Headers': 'Content-Type, Authorization'
    });
  }

  if (parsed.pathname === '/proxy') {
    const target = parsed.query.url;
    if (!target) return send(res, 400, 'url required');
    return proxy(req, res, target);
  }

  return serveStatic(req, res);
});

server.listen(PORT, () => {
  console.log(`Server running at http://localhost:${PORT}`);
});
