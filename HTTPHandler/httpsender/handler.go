package httpsender

import (
	"encoding/base64"
	"encoding/json"
	"fmt"
	"image/jpeg"
	"io"
	"io/ioutil"
	"log"
	"net/http"
	"net/url"
	"os"
	"strconv"
	"strings"

	"github.com/EdasL/HTTPHandler/database"

	"github.com/julienschmidt/httprouter"
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
	UploadURL    string `yaml:"UploadURL"`
}
type TrainRequest struct {
	Album    string `json:"album"`
	AlbumKey string `json:"albumkey"`
	EntryID  string `json:"entryid"`
	Urls     string `json:"urls"`
}

type RecogniseRequest struct {
	Album    string `json:"album"`
	AlbumKey string `json:"albumkey"`
	Urls     string `json:"urls"`
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
type UpdateRequest struct {
	Email string `json:"email"`
	Image string `json:"picture"`
}
type Email struct {
	Email string `json:"email"`
}
type Handler struct {
	Router *httprouter.Router

	DB database.DB

	Config *Config
}

func NewHandler(path string) *Handler {
	h := &Handler{
		Router: httprouter.New(),
	}

	ConfigInst, err := LoadConfigFile(path)
	if err != nil {
		log.Fatal(err)
	}

	h.Config = ConfigInst

	h.Router.POST("/train", h.TrainHandler)
	h.Router.POST("/rec", h.RecHandler)
	h.Router.GET("/pic", h.UploadHandler)
	h.Router.POST("/email", h.EmailHandler)
	h.Router.POST("/register", h.RegisterHandler)
	h.Router.POST("/login", h.LoginHandler)
	h.Router.GET("/list", h.ListHandler)
	h.Router.POST("/user", h.UserHandler)
	h.Router.POST("/update", h.UpdateHandler)

	return h
}
func (h *Handler) UpdateHandler(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	log.Println("Update user handling...")

	var req UpdateRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		w.Write([]byte("Cannot decode request JSON"))
		log.Println("Cannot decode request JSON", err)
		return
	}
	user := database.Human{
		Email: req.Email,
		Photo: req.Image,
	}
	if err := h.DB.Update(user); err != nil {
		w.Write([]byte("Cannot update user -" + err.Error()))
		log.Println("Cannot update user -" + err.Error())
		return
	}

	w.Write([]byte("200"))
	log.Println("Success")

}
func (h *Handler) UserHandler(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	log.Println("Get user handling...")

	var req Email
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		w.Write([]byte("Cannot decode request JSON"))
		log.Println("Cannot decode request JSON", err)
		return
	}

	user := h.DB.Email(req.Email)
	if user == nil {
		w.Write([]byte("Couldn't get the user from DB"))
		log.Println("Couldn't get the user from DB")
		return
	}

	userbytes, err := json.Marshal(user)
	if err != nil {
		w.Write([]byte("Failed to marshal user to bytes"))
		log.Println("Failed to marshal user to bytes -" + err.Error())
		return
	}
	w.Write(userbytes)

}
func (h *Handler) ListHandler(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	users, err := h.DB.ListUsers()
	usersbytes, err := json.Marshal(users)
	if err != nil {
		w.Write([]byte("Error: " + err.Error()))
	}
	w.Write(usersbytes)
}

func (h *Handler) LoginHandler(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	log.Println("Login handling...")
	if r.Body == nil {
		w.Write([]byte("Empty request"))
		log.Fatal("Empty request")
	}
	var req database.Human
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		w.Write([]byte("Cannot decode request JSON " + err.Error()))
		log.Println("Cannot decode request JSON ", err)
	}

	if err := h.DB.Login(req); err != nil {
		w.Write([]byte("Incorrect username or password" + err.Error()))
		log.Println(err.Error())
	} else {
		w.Write([]byte("200"))
	}
}

func (h *Handler) RegisterHandler(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	log.Println("Register handling...")
	if r.Body == nil {
		w.Write([]byte("Empty request"))
		log.Fatal("Empty request")
	}

	var req database.Human
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		w.Write([]byte("Cannot decode request JSON " + err.Error()))
		log.Println("Cannot decode request JSON ", err)
	}

	if err := h.DB.Create(req); err != nil {
		w.Write([]byte(err.Error()))
		log.Println("Cannot create user ", err)
	} else {
		log.Println("Success")
		w.Write([]byte("200"))
	}
}

func (h *Handler) EmailHandler(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	log.Println("Email handling...")

	if r.Body == nil {
		w.Write([]byte("Empty request"))
		log.Fatal("Empty request")
	}

	var req database.Human
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		w.Write([]byte("Cannot decode request JSON " + err.Error()))
		log.Println("Cannot decode request JSON ", err)
	}

	user := h.DB.Email(req.Email)

	if user != nil {
		w.Write([]byte("Email exists"))
	} else {
		w.Write([]byte("200"))
	}
}

func (h *Handler) TrainHandler(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	fmt.Println("train request handling...")

	if r.Body == nil {
		w.Write([]byte("Empty request"))
		log.Fatal("Empty request")
	}
	if err := r.ParseForm(); err != nil {
		w.Write([]byte("Couldn't parse request form " + err.Error()))
		log.Fatal("Couldn't parse request form", err)
	}
	if r.Form == nil {
		w.Write([]byte("Empty request form"))
		log.Fatal("Empty request form")
	}

	formdata := make(map[string]string)
	for key, data := range r.Form {
		formdata[key] = strings.Join(data, "")
	}

	SavePic(formdata["files"])

	id := strings.Replace(formdata["entryid"], "@", "b24c6hars", -1)
	id = strings.Replace(id, ".", "b34t7task", -1)
	id = strings.Replace(id, "-", "b43c8bruk", -1)
	data := url.Values{}
	data.Set("album", h.Config.Album)
	data.Set("albumkey", h.Config.AlbumKey)
	data.Set("entryid", id)
	data.Set("urls", h.Config.UploadURL)

	resp, err := Send(*h.Config, "POST", strings.NewReader(data.Encode()), h.Config.TrainURL)
	body, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		w.Write([]byte("Failed to read from response body stream" + err.Error()))
	}
	if err != nil && resp.StatusCode != 200 {
		fmt.Println("ERROR " + strconv.Itoa(resp.StatusCode) + string(body))
		w.Write([]byte("ERROR " + strconv.Itoa(resp.StatusCode) + string(body)))
	} else {
		fmt.Println(strconv.Itoa(resp.StatusCode) + string(body))
		w.Write([]byte(strconv.Itoa(resp.StatusCode) + string(body)))
	}
	resp.Body.Close()
	body = Rebuild(*h.Config)
	log.Println(string(body))
	w.Write(body)

}
func (h *Handler) RecHandler(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	fmt.Println("recognition request handling...")

	if r.Body == nil {
		w.Write([]byte("Empty request"))
		log.Fatal("Empty request")
	}
	if err := r.ParseForm(); err != nil {
		w.Write([]byte("Couldn't parse request form " + err.Error()))
		log.Fatal("Couldn't parse request form", err)
	}
	if r.Form == nil {
		w.Write([]byte("Empty request form"))
		log.Fatal("Empty request form")
	}

	formdata := make(map[string]string)
	for key, data := range r.Form {
		formdata[key] = strings.Join(data, "")
	}
	SavePic(formdata["files"])
	data := url.Values{}
	data.Set("album", h.Config.Album)
	data.Add("albumkey", h.Config.AlbumKey)
	data.Add("urls", h.Config.UploadURL)

	resp, err := Send(*h.Config, "POST", strings.NewReader(data.Encode()), h.Config.RecognizeURL)
	defer resp.Body.Close()
	if err != nil || resp.StatusCode != 200 {
		w.Write([]byte("ERROR " + string(resp.StatusCode)))
	} else {
		body, err := ioutil.ReadAll(resp.Body)
		if err != nil {
			w.Write([]byte("Failed to read from response body stream" + err.Error()))
		}

		var respObject Recognition
		err = json.Unmarshal(body, &respObject)
		if err != nil {
			w.Write([]byte("Failed to decode response JSON" + err.Error()))
		} else {
			id := FindID(respObject)
			id = strings.Replace(id, "b24c6hars", "@", -1)
			id = strings.Replace(id, "b34t7task", ".", -1)
			id = strings.Replace(id, "b43c8bruk", "-", -1)

			user := h.DB.Email(id)
			if user == nil {
				w.Write([]byte("User doesn't exists"))
				log.Println("User doesn't exists")
			} else {
				userbytes, err := json.Marshal(user)
				if err != nil {
					w.Write([]byte("Failed to marshal user to bytes -" + err.Error()))
					log.Println("Failed to marshal user to bytes -" + err.Error())
				}
				w.Write(userbytes)
			}
		}
	}
	defer resp.Body.Close()
}

func (h *Handler) UploadHandler(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	fmt.Println("Uploading picture...")
	img, err := os.Open("upload.jpg")
	if err != nil {
		w.Write([]byte(err.Error()))
		log.Fatal(err)
	}
	defer img.Close()
	w.Header().Set("Content-Type", "image/jpeg")
	io.Copy(w, img)
}
func SavePic(data string) {
	data = strings.Replace(data, " ", "+", -1)

	reader := base64.NewDecoder(base64.StdEncoding, strings.NewReader(data))
	img, err := jpeg.Decode(reader)
	if err != nil {
		log.Fatal("Couldn't decode jpeg -", err)
	}
	//save the imgByte to file
	out, err := os.Create("./upload.jpg")
	if err != nil {
		log.Fatal("Couldn't create .jpg file", err)
	}

	err = jpeg.Encode(out, img, nil)
	if err != nil {
		log.Fatal("Couldn't encode image to file", err)
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

func FindID(resp Recognition) string {
	var max float64 = 0
	var ID string
	for i := 0; i < len(resp.Photos); i++ {
		for j := 0; j < len(resp.Photos[i].Tags); j++ {
			for _, UID := range resp.Photos[i].Tags[j].Uids {
				if UID.Confidence > max {
					max = UID.Confidence
					ID = UID.Prediction
				}
			}
		}
	}
	return ID
}
