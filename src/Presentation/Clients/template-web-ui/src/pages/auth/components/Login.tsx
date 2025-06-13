import { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { Segment, Form, Button } from "semantic-ui-react";
import { useNavigate } from "react-router-dom";
import { useAppDispatch } from "../../../hooks";
import { login } from "../../../features/authSlice";
import { useSelector } from "react-redux";
import { RootState } from "../../../store";

interface LoginProps {
  toggleForm: () => void;
}

const Login: React.FC<LoginProps> = ({ toggleForm }) => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [emailOrUsername, setEmailOrUsername] = useState("");
  const [password, setPassword] = useState("");

  const { loading, isAuthenticated } = useSelector(
    (state: RootState) => state.auth
  );

  useEffect(() => {
    if (isAuthenticated) {
      navigate("/");
    }
  }, [isAuthenticated, navigate]);

  const handleLogin = async () => {
    dispatch(login({ usernameOrEmail: emailOrUsername, password }));
  };

  return (
    <Segment raised padded="very" textAlign="center">
      <h2>{t("login")}</h2>
      <Form>
        <Form.Input
          fluid
          icon="user"
          iconPosition="left"
          placeholder={t("usernameOrEmail")}
          value={emailOrUsername}
          onChange={(e) => setEmailOrUsername(e.target.value)}
        />
        <Form.Input
          fluid
          icon="lock"
          iconPosition="left"
          placeholder={t("password")}
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        <Button color="blue" fluid loading={loading} onClick={handleLogin}>
          {t("login")}
        </Button>
      </Form>
      <p style={{ marginTop: "10px" }}>
        {t("noAccount")}{" "}
        <span style={{ color: "blue", cursor: "pointer" }} onClick={toggleForm}>
          {t("register")}
        </span>
      </p>
    </Segment>
  );
};

export default Login;
