package httpsender

import (
	"fmt"
	"io"
	"io/ioutil"
	"log"
	"net/http"
)

//Send sends http request to api
func Send(config Config, request string, body io.Reader, url string) (*http.Response, error) {
	req, err := http.NewRequest(request, url, body)
	if err != nil {
		return nil, err
	}
	req.Header.Add("X-Mashape-Key", config.MashapeKey)
	req.Header.Add("Content-Type", "application/x-www-form-urlencoded")
	req.Header.Add("Accept", "application/json")

	resp, err := http.DefaultClient.Do(req)
	if err != nil {
		return nil, err
	}

	return resp, nil
}

// Rebuild rebuilds created albums after training
func Rebuild(config Config) {
	req, err := http.NewRequest("GET", config.RebuildURL, nil)
	if err != nil {
		log.Println(err)
	}
	req.Header.Set("X-Mashape-Key", config.MashapeKey)

	resp, err := http.DefaultClient.Do(req)
	if err != nil {
		log.Println(err)
	}
	body, _ := ioutil.ReadAll(resp.Body)
	fmt.Println(string(body))
	defer resp.Body.Close()

}
