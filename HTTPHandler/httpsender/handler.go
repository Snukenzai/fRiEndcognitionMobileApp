package httpsender

import (
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"net/url"
	"strconv"
	"strings"

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
type Recognition struct {
	Status string   `json:"status"`
	Images []string `json:"images"`
	Photos []struct {
		URL   string `json:"url"`
		Width int    `json:"width"`
		Tags  []struct {
			EyeLeft struct {
				Y int `json:"y"`
				X int `json:"x"`
			} `json:"eye_left"`
			Confidence float64 `json:"confidence"`
			Center     struct {
				Y int `json:"y"`
				X int `json:"x"`
			} `json:"center"`
			MouthRight struct {
				Y float64 `json:"y"`
				X float64 `json:"x"`
			} `json:"mouth_right"`
			MouthLeft struct {
				Y float64 `json:"y"`
				X float64 `json:"x"`
			} `json:"mouth_left"`
			Height      int `json:"height"`
			Width       int `json:"width"`
			MouthCenter struct {
				Y float64 `json:"y"`
				X float64 `json:"x"`
			} `json:"mouth_center"`
			Nose struct {
				Y int `json:"y"`
				X int `json:"x"`
			} `json:"nose"`
			EyeRight struct {
				Y int `json:"y"`
				X int `json:"x"`
			} `json:"eye_right"`
			Tid        string `json:"tid"`
			Attributes []struct {
				SmileRating float64 `json:"smile_rating"`
				Smiling     bool    `json:"smiling"`
				Confidence  float64 `json:"confidence"`
			} `json:"attributes"`
			Uids []struct {
				Confidence float64 `json:"confidence"`
				Prediction string  `json:"prediction"`
				UID        string  `json:"uid"`
			} `json:"uids"`
		} `json:"tags"`
		Height int `json:"height"`
	} `json:"photos"`
}

func TrainHandler(w http.ResponseWriter, r *http.Request) {
	if r.Body == nil {
		log.Fatal("Empty request")
	}

	ConfigInst, err := LoadConfigFile("./httpsender/config.yaml")
	if err != nil {
		log.Fatal(err)
	}
	var req TrainRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		log.Fatal("Cannot decode request JSON")
	}
	req.Album = ConfigInst.Album
	req.AlbumKey = ConfigInst.AlbumKey

	fmt.Println(req)

	data := url.Values{}
	data.Set("album", req.Album)
	data.Add("albumkey", req.AlbumKey)
	data.Add("entryid", req.EntryID)
	data.Add("files", string(req.File))

	fmt.Println(strings.NewReader(data.Encode()))
	resp, err := Send(*ConfigInst, "POST", strings.NewReader(data.Encode()), ConfigInst.TrainURL)
	if err != nil && resp.StatusCode != 200 {
		w.Write([]byte(strconv.Itoa(resp.StatusCode)))
	} else {
		w.Write([]byte(strconv.Itoa(resp.StatusCode)))
	}
	resp.Body.Close()
	Rebuild(*ConfigInst)

}
func RecHandler(w http.ResponseWriter, r *http.Request) {
	if r.Body == nil {
		log.Fatal("Empty request")
	}
	ConfigInst, err := LoadConfigFile("./config.yaml")
	if err != nil {
		log.Fatal(err)
	}

	var req RecogniseRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		log.Println("Cannot decode request JSON")
	}
	req.Album = ConfigInst.Album
	req.AlbumKey = ConfigInst.AlbumKey

	data := url.Values{}
	data.Set("album", req.Album)
	data.Add("albumkey", req.AlbumKey)
	data.Add("files", string(req.File))

	resp, err := Send(*ConfigInst, "POST", strings.NewReader(data.Encode()), ConfigInst.RecognizeURL)
	defer resp.Body.Close()
	if err != nil || resp.StatusCode != 200 {
		w.Write([]byte("ERROR " + string(resp.StatusCode)))
	} else {
		body, err := ioutil.ReadAll(resp.Body)
		if err != nil {
			w.Write([]byte("Failed to read from response body stream"))
		}

		var respObject Recognition
		err = json.Unmarshal(body, &respObject)
		if err != nil {
			w.Write([]byte("Failed to decode response JSON"))
		} else {
			id, proc := FindID(respObject)
			w.Write([]byte(id + " " + proc))
		}
	}
	defer resp.Body.Close()

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

func FindID(resp Recognition) (string, string) {
	max := strconv.Itoa(0)
	var ID string
	for i := 0; i < len(resp.Photos); i++ {
		for j := 0; j < len(resp.Photos[i].Tags); j++ {
			for _, UID := range resp.Photos[i].Tags[j].Uids {
				if UID.Prediction > max {
					max = UID.Prediction
					ID = UID.UID
				}
			}
		}
	}
	return ID, max
}
