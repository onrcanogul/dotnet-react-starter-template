import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Segment, Form, Button } from "semantic-ui-react";
import { useNavigate } from "react-router-dom";
import { isAuthenticated, login } from "../services/auth-services";

interface LoginProps {
  toggleForm: () => void;
}

const Login: React.FC<LoginProps> = ({ toggleForm }) => {
  const { t } = useTranslation();
  const navigate = useNavigate();

  const [emailOrUsername, setEmailOrUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleLogin = async () => {
    if (!isAuthenticated) return;

    setLoading(true);
    setError(null);

    try {
      const success = await login(emailOrUsername, password);
      if (success) {
        setTimeout(() => {
          navigate("/");
        }, 1500);
      } else {
        setError(t("errorLogin"));
      }
    } catch (error) {
      setError(t("errorLogin"));
    } finally {
      setLoading(false);
    }
  };

  return (
    <Segment raised padded="very" textAlign="center">
      <h2>{t("login")}</h2>
      {error && <p style={{ color: "red" }}>{error}</p>}
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
