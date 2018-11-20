package httpsender

import (
	"bytes"
	"encoding/json"
	"io/ioutil"
	"log"
	"net/http"

	"github.com/pkg/errors"
	yaml "gopkg.in/yaml.v2"
)

type Config struct {
	Album        string `yaml:"Album"`
	AlbumKey     string `yaml:"AlbumKey"`
	MashapeKey   string `yaml:"MashapeKey"`
	RecognizeURL string `yaml:"RecognizeURL"`
	TrainURL     string `yaml:"TrainURL"`
	RebuildURL   string `yaml:"RebuildURL"`
}
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

func TrainHandler(w http.ResponseWriter, r *http.Request) {
	ConfigInst, err := LoadConfigFile("config.yaml")
	if err != nil {
		log.Fatal(err)
	}
	var req TrainRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		log.Println("Cannot decode request JSON")
	}
	req.Album = ConfigInst.Album
	req.AlbumKey = ConfigInst.AlbumKey

	jsonValue, err := json.Marshal(req)
	if err != nil {
		log.Println("Cannot marshal to JSON")
	}
	resp, err := Send(*ConfigInst, "POST", bytes.NewBuffer(jsonValue), ConfigInst.TrainURL)
	if err != nil {
		w.Write([]byte("ERROR" + string(resp.StatusCode)))
	} else {
		w.Write([]byte("Status OK" + string(resp.StatusCode)))
	}
	Rebuild(*ConfigInst)

}
func RecHandler(w http.ResponseWriter, r *http.Request) {
	ConfigInst, err := LoadConfigFile("config.yaml")
	if err != nil {
		log.Fatal(err)
	}
	var req RecogniseRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		log.Println("Cannot decode request JSON")
	}
	req.Album = ConfigInst.Album
	req.AlbumKey = ConfigInst.AlbumKey

	jsonValue, err := json.Marshal(req)
	if err != nil {
		log.Println("Cannot marshal to JSON")
	}
	resp, err := Send(*ConfigInst, "POST", bytes.NewBuffer(jsonValue), ConfigInst.RecognizeURL)
	if err != nil {
		w.Write([]byte("ERROR" + string(resp.StatusCode)))
	} else {
		w.Write([]byte("Status OK" + string(resp.StatusCode)))
	}

}
func LoadConfigFile(path string) (*Config, error) {
	configFile, err := ioutil.ReadFile(path)
	if err != nil {
		return nil, errors.Wrapf(err, "can't open the config file- %v", path)
	}

	var config Config
	err = yaml.Unmarshal(configFile, &config)
	if err != nil {
		return nil, errors.Wrapf(err, "can't read the alerts file- %v", path)
	}

	return &config, nil
}
