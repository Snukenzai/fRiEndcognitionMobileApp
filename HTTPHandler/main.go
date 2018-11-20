package main

import (
	"log"
	"net/http"

	"github.com/EdasL/HTTPHandler/httpsender"
)

func main() {
	http.HandleFunc("/train", httpsender.TrainHandler)
	http.HandleFunc("/rec", httpsender.RecHandler)
	http.HandleFunc("/db", httpsender.DBHandler)

	if err := http.ListenAndServe(":8080", nil); err != nil {
		log.Fatal(err)
	}
}
