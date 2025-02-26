import React from "react";
import { Form, Button, Segment } from "semantic-ui-react";

interface RegisterProps {
  toggleForm: () => void;
}

const Register: React.FC<RegisterProps> = ({ toggleForm }) => {
  return (
    <Segment raised padded="very" textAlign="center">
      <h2>Kayıt Ol</h2>
      <Form>
        <Form.Input
          fluid
          icon="user"
          iconPosition="left"
          placeholder="Adınız"
        />
        <Form.Input
          fluid
          icon="mail"
          iconPosition="left"
          placeholder="E-posta"
        />
        <Form.Input
          fluid
          icon="lock"
          iconPosition="left"
          placeholder="Şifre"
          type="password"
        />
        <Button color="green" fluid>
          Kayıt Ol
        </Button>
      </Form>
      <p style={{ marginTop: "10px" }}>
        Zaten bir hesabınız var mı?{" "}
        <span
          style={{ color: "green", cursor: "pointer" }}
          onClick={toggleForm}
        >
          Giriş Yap
        </span>
      </p>
    </Segment>
  );
};

export default Register;
