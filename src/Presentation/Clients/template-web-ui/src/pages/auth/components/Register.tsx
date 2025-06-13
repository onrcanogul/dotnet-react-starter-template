import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { Form, Button, Segment } from "semantic-ui-react";
import { register } from "../services/auth-services";
import ToastrService from "../../../utils/toastr";

interface RegisterProps {
  toggleForm: () => void;
}

const Register: React.FC<RegisterProps> = ({ toggleForm }) => {
  const { t } = useTranslation();
  const [formData, setFormData] = useState({
    username: "",
    fullName: "",
    email: "",
    password: "",
    confirmPassword: "",
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async () => {
    if (formData.password !== formData.confirmPassword) {
      ToastrService.warning(t("passwordMismatch"));
      return;
    }

    const result = await register(
      formData.username,
      formData.fullName,
      formData.email,
      formData.password,
      formData.confirmPassword
    );
    if (result) {
      setTimeout(() => {
        toggleForm();
      }, 1500);
    }
  };

  return (
    <Segment raised padded="very" textAlign="center">
      <h2>{t("register")}</h2>
      <Form onSubmit={handleSubmit}>
        <Form.Input
          fluid
          icon="user circle"
          iconPosition="left"
          placeholder={t("username")}
          name="username"
          value={formData.username}
          onChange={handleChange}
        />
        <Form.Input
          fluid
          icon="user"
          iconPosition="left"
          placeholder={t("fullName")}
          name="fullName"
          value={formData.fullName}
          onChange={handleChange}
        />
        <Form.Input
          fluid
          icon="mail"
          iconPosition="left"
          placeholder={t("email")}
          name="email"
          value={formData.email}
          onChange={handleChange}
        />
        <Form.Input
          fluid
          icon="lock"
          iconPosition="left"
          placeholder={t("password")}
          type="password"
          name="password"
          value={formData.password}
          onChange={handleChange}
        />
        <Form.Input
          fluid
          icon="lock"
          iconPosition="left"
          placeholder={t("confirmPassword")}
          type="password"
          name="confirmPassword"
          value={formData.confirmPassword}
          onChange={handleChange}
        />
        <Button color="green" fluid type="submit">
          {t("register")}
        </Button>
      </Form>
      <p style={{ marginTop: "10px" }}>
        {t("alreadyHaveAccount")}{" "}
        <span
          style={{ color: "green", cursor: "pointer" }}
          onClick={toggleForm}
        >
          {t("login")}
        </span>
      </p>
    </Segment>
  );
};

export default Register;
