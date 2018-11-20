package main

import (
	"encoding/json"
	"log"
	"net/http"
)

func main() {
	http.HandleFunc("/train", trainHandler)
	http.HandleFunc("/rec", recHandler)
	http.HandleFunc("/pic", picHandler)

	if err := http.ListenAndServe(":8080", nil); err != nil {
		log.Fatal(err)
	}

}

func trainHandler(w http.ResponseWriter, r *http.Request) {
	enc := json.NewEncoder(w)
	enc.Encode(r.Body)

	w.Write([]byte("Got it"))
}
func recHandler(w http.ResponseWriter, r *http.Request) {
	enc := json.NewEncoder(w)
	enc.Encode(r.Body)

	w.Write([]byte("Got it"))
}
func picHandler(w http.ResponseWriter, r *http.Request) {
	enc := json.NewEncoder(w)
	enc.Encode(r.Body)

	w.Write([]byte("Got it"))
}
