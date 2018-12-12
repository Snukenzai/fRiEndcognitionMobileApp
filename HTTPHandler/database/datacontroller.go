package database

import (
	"encoding/json"
	"fmt"
)

type DB struct {
	client *Client
}
type Human struct {
	Email    string `json:"email"`
	Name     string `json:"name"`
	Surname  string `json:"surname"`
	Password string `json:"password"`
	Photo    string `json:"picture"`
}

func (s *DB) Email(email string) *Human {
	tx, err := s.client.db.Begin(false)
	if err != nil {
		return nil
	}
	defer tx.Rollback()

	v := tx.Bucket([]byte("Faces")).Get([]byte(email))
	if v == nil {
		return nil
	}

	var h Human

	if err := json.Unmarshal(v, &h); err != nil {
		return nil
	}

	return &h
}

func (s *DB) Create(h Human) error {
	tx, err := s.client.db.Begin(true)
	if err != nil {
		return err
	}
	defer tx.Rollback()

	b := tx.Bucket([]byte("Faces"))

	humanbytes, err := json.Marshal(h)
	if err != nil {
		return err
	}

	err = b.Put([]byte(h.Email), humanbytes)
	if err != nil {
		return err
	}

	return tx.Commit()
}

func (s *DB) Update(h Human) error {
	tx, err := s.client.db.Begin(true)
	if err != nil {
		return err
	}
	defer tx.Rollback()

	b := tx.Bucket([]byte("Faces"))

	v := b.Get([]byte(h.Email))

	if v == nil {
		return fmt.Errorf("User by this email not found")
	}

	var hfromDB Human

	if err := json.Unmarshal(v, &hfromDB); err != nil {
		return err
	}

	hfromDB.Photo = h.Photo

	userbytes, err := json.Marshal(hfromDB)
	if err != nil {
		return err
	}
	// Marshal and insert record.
	if err := b.Put([]byte(h.Email), userbytes); err != nil {
		return err
	}

	return tx.Commit()
}

func (s *DB) Login(h Human) error {
	tx, err := s.client.db.Begin(false)
	if err != nil {
		return err
	}
	defer tx.Rollback()

	v := tx.Bucket([]byte("Faces")).Get([]byte(h.Email))
	if v == nil {
		return fmt.Errorf("User doesn't exist")
	}

	var dbh Human

	if err := json.Unmarshal(v, &dbh); err != nil {
		return err
	}

	if dbh.Password != h.Password {
		return fmt.Errorf("Incorrect password")
	}
	return nil
}

func (s *DB) ListUsers() ([]Human, error) {
	var h []Human
	var human Human
	tx, err := s.client.db.Begin(false)
	if err != nil {
		return nil, err
	}
	defer tx.Rollback()
	v := tx.Bucket([]byte("Faces"))

	if err := v.ForEach(func(k, v []byte) error {
		if err = json.Unmarshal(v, &human); err != nil {
			return err
		}
		h = append(h, human)
		return nil
	}); err != nil {
		return nil, err
	}

	return h, nil

}
