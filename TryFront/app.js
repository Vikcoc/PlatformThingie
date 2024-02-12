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
app.get("/bundle", function (req, res) {
    res.sendFile(path.join(__dirname, 'scripts', 'bundle.js'));
});

app.get("/style", function (req, res) {
    res.sendFile(path.join(__dirname, 'styles', 'style.css'));
});

app.get("/color", function (req, res) {
    res.sendFile(path.join(__dirname, 'styles', 'color.css'));
});

app.get("/favicon.ico", function (req, res) {
    res.sendFile(path.join(__dirname,'modsig.svg'));
});


app.get("/about", function (req, res) {
    res.sendFile(path.join(__dirname, 'pages', 'topbar.html'));
});
app.get("/about/bundle", function (req, res) {
    res.sendFile(path.join(__dirname, 'scripts', 'bundle.js'));
});

app.get("/about/style", function (req, res) {
    res.sendFile(path.join(__dirname, 'styles', 'topbar.css'));
});


app.listen(port, function () {
  console.log(`Example app listening on port ${port}!`);
});
