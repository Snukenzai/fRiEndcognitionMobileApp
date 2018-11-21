package main

import (
	"flag"
	"log"
	"net/http"

	"github.com/EdasL/HTTPHandler/httpsender"
)

func main() {
	http.HandleFunc("/train", httpsender.TrainHandler)
	http.HandleFunc("/rec", httpsender.RecHandler)

	url := flag.String("url", "0.0.0.0:8080", "an export url path")
	flag.Parse()
	if err := http.ListenAndServe(*url, nil); err != nil {
		log.Fatal(err)
	}
}
