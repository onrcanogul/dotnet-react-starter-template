import React from "react";
import { Form, Button, Segment } from "semantic-ui-react";

interface LoginProps {
  toggleForm: () => void;
}

const Login: React.FC<LoginProps> = ({ toggleForm }) => {
  return (
    <Segment raised padded="very" textAlign="center">
      <h2>Giriş Yap</h2>
      <Form>
        <Form.Input
          fluid
          icon="user"
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
        <Button color="blue" fluid>
          Giriş Yap
        </Button>
      </Form>
      <p style={{ marginTop: "10px" }}>
        Hesabınız yok mu?{" "}
        <span style={{ color: "blue", cursor: "pointer" }} onClick={toggleForm}>
          Kayıt Ol
        </span>
      </p>
    </Segment>
  );
};

export default Login;
