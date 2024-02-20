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

app.get("/signIn.js", function (req, res) {
    res.sendFile(path.join(__dirname, 'scripts', 'signIn.js'));
});

app.get("/style", function (req, res) {
    res.sendFile(path.join(__dirname, 'styles', 'style.css'));
});

app.get("/color", function (req, res) {
    res.sendFile(path.join(__dirname, 'styles', 'color.css'));
});
app.get("/font", function (req, res) {
    res.sendFile(path.join(__dirname, 'styles', 'font.css'));
});

app.get("/favicon.ico", function (req, res) {
    res.sendFile(path.join(__dirname,'modsig.svg'));
});



app.listen(port, function () {
  console.log(`Example app listening on port ${port}!`);
});
