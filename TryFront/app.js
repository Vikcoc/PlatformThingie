const express = require("express");
const app = express();
const port = 3000;
const path = require('path');

app.use((req, res, next) => {
    console.log(req.path)
    next()
})

app.get("/", function (req, res) {
    res.sendFile(path.join(__dirname, 'pages', 'index.html'));
});
app.get("/index.js", function (req, res) {
    res.sendFile(path.join(__dirname, 'scripts', 'index.js'));
});
app.get("/bundle.js", function (req, res) {
    res.sendFile(path.join(__dirname, 'bundle.js'));
});
app.get("/filled-button", function (req, res) {
    res.sendFile(path.join(__dirname, 'node_modules', '@material', 'web', 'button', 'filled-button.js'));
});

app.listen(port, function () {
  console.log(`Example app listening on port ${port}!`);
});
