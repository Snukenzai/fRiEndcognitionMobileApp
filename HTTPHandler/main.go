package main

import (
	"flag"
	"log"
	"net/http"

	"github.com/EdasL/HTTPHandler/database"
	"github.com/EdasL/HTTPHandler/httpsender"
)

func main() {
	url := flag.String("url", "0.0.0.0:80", "an export url path")
	configpath := flag.String("path", "./config.yaml", "config file path")
	flag.Parse()

	Client := database.NewClient()
	Client.Path = "./mordos.db"
	err := Client.Open()
	if err != nil {
		log.Fatal(err)
	}
	defer Client.Close()

	dataCtr := Client.DataController()

	handler := httpsender.NewHandler(*configpath)
	handler.DB = *dataCtr

	if err := http.ListenAndServe(*url, handler.Router); err != nil {
		log.Fatal(err)
	}
}
