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

app.get("/public/material-components", function (req, res) {
    res.sendFile(path.join(__dirname, 'scripts', 'material-components.js'));
});

app.get("/public/authenticated-fetch", function (req, res) {
    res.sendFile(path.join(__dirname, 'scripts', 'authenticated-fetch.js'));
});

app.get("/user/user-info", function (req, res) {
    res.sendFile(path.join(__dirname, 'scripts', 'user-info.js'));
});

app.get("/public/signed-in", function (req, res) {
    res.sendFile(path.join(__dirname, 'scripts', 'signed-in.js'));
});

app.get("/public/style", function (req, res) {
    res.sendFile(path.join(__dirname, 'styles', 'style.css'));
});
app.get("/login/style", function (req, res) {
    res.sendFile(path.join(__dirname, 'styles', 'login.css'));
});


app.get("/public/color", function (req, res) {
    res.sendFile(path.join(__dirname, 'styles', 'color.css'));
});
app.get("/public/font", function (req, res) {
    res.sendFile(path.join(__dirname, 'styles', 'font.css'));
});

app.get("/public/topbar", function (req, res) {
    res.sendFile(path.join(__dirname, 'styles', 'topbar.css'));
});


app.get("/favicon.ico", function (req, res) {
    res.sendFile(path.join(__dirname,'modsig.svg'));
});

app.get("/public/back-logo", function (req, res) {
    res.sendFile(path.join(__dirname, 'pictures', 'back-logo.svg'));
});
app.get("/public/save-logo", function (req, res) {
    res.sendFile(path.join(__dirname, 'pictures', 'save-logo.svg'));
});
app.get("/public/plus-logo", function (req, res) {
    res.sendFile(path.join(__dirname, 'pictures', 'plus-logo.svg'));
});
app.get("/public/trashcan-logo", function (req, res) {
    res.sendFile(path.join(__dirname, 'pictures', 'trashcan-logo.svg'));
});



app.listen(port, function () {
  console.log(`Example app listening on port ${port}!`);
});
