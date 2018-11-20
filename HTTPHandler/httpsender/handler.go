package httpsender

import (
	"encoding/json"
	"net/http"
)

type TrainRequest struct {
	Album    string `json:"album"`
	AlbumKey string `json:"albumkey"`
	EntryID  string `json:"entryid"`
	File     []byte `json:"files"`
}

type RecogniseRequest struct {
	Album    string `json:"album"`
	AlbumKey string `json:"albumkey"`
	File     []byte `json:"files"`
}

type DBRequest struct {
}

func TrainHandler(w http.ResponseWriter, r *http.Request) {
	enc := json.NewEncoder(w)
	enc.Encode(r.Body)

	w.Write([]byte("Got it"))
}
func RecHandler(w http.ResponseWriter, r *http.Request) {
	enc := json.NewEncoder(w)
	enc.Encode(r.Body)

	w.Write([]byte("Got it"))
}
func PicHandler(w http.ResponseWriter, r *http.Request) {
	enc := json.NewEncoder(w)
	enc.Encode(r.Body)

	w.Write([]byte("Got it"))
}
func DBHandler(w http.ResponseWriter, r *http.Request) {
	enc := json.NewEncoder(w)
	enc.Encode(r.Body)

	w.Write([]byte("Got it"))
}
