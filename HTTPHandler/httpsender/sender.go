package httpsender

import (
	"io"
	"log"
	"net/http"
)

//Send sends http request to api
func Send(config Config, request string, body io.Reader, url string) (*http.Response, error) {
	client := http.Client{}

	req, err := http.NewRequest(request, url, body)
	if err != nil {
		return nil, err
	}
	req.Header.Set("X-Mashape_key", config.MashapeKey)
	req.Header.Set("Content-Type", "application/x-www-form-urlencoded")
	req.Header.Set("Accept", "application/json")

	resp, err := client.Do(req)
	if err != nil {
		return nil, err
	}

	return resp, nil
}

// Rebuild rebuilds created albums after training
func Rebuild(config Config) {
	client := http.Client{}

	req, err := http.NewRequest("GET", config.RebuildURL, nil)
	if err != nil {
		log.Println(err)
	}
	req.Header.Set("X-Mashape_key", config.MashapeKey)
	req.Header.Set("Accept", "application/json")

	_, err = client.Do(req)
	if err != nil {
		log.Println(err)
	}

}
