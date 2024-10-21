import styled from "styled-components";
import useToggle from "hooks/useToggle";
import { useForm } from "react-hook-form";
import { useAgent } from "data/user/useAgent";

import mainLogo from "images/main_logo.png";
import Input from "components/input/Input";
import ErrorMessage from "components/ErrorMessage";

export interface ILoginForm {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export default function AuthPage() {
  const { login, authError } = useAgent();

  const [isPasswordShown, setIsPasswordShown] = useToggle();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ILoginForm>();

  return (
    <AuthWrapper>
      <AuthForm>
        <LogoContainer>
          <Logo src={mainLogo} alt="aorta" />
          <EnvContainer>
            <Side>Agent</Side>
            <Env env={process.env.REACT_APP_ENV}>{process.env.REACT_APP_ENV}</Env>
          </EnvContainer>
        </LogoContainer>
        <form onSubmit={handleSubmit(login)}>
          <Label>
            <InputWrapper>
              <Input
                inputIcon={<i className="icon-ic-user" />}
                {...register("email", {
                  required: true,
                })}
                autoComplete="on"
                type="email"
                placeholder="Email"
                name="email"
                data-testid="email"
              />
            </InputWrapper>
            <InputWrapper>
              <ErrorMessage>
                {errors.email && "Email is required and should be in a valid format"}
              </ErrorMessage>
            </InputWrapper>
          </Label>
          <Label>
            <InputWrapper>
              <Input
                {...register("password", {
                  required: true,
                })}
                inputIcon={<i className="icon-ic-lock" />}
                autoComplete="on"
                type={isPasswordShown ? "text" : "password"}
                placeholder="Password"
                name="password"
                data-testid="password"
              />
              <i className="icon-ic-eye" onClick={setIsPasswordShown} />
            </InputWrapper>
            <InputWrapper>
              <ErrorMessage>{errors.password && "Password is requeired field"}</ErrorMessage>
            </InputWrapper>
          </Label>
          <StyledButton data-testid="login-button">Login</StyledButton>
          <InputWrapper>
            <ErrorMessage>{authError}</ErrorMessage>
          </InputWrapper>
        </form>
      </AuthForm>
    </AuthWrapper>
  );
}

const AuthWrapper = styled.div`
  width: 100vw;
  height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background-color: ${({ theme }) => theme.colors.bg.primary};
`;

const AuthForm = styled.div`
  background-color: ${({ theme }) => theme.colors.bg.secondary};
  padding: 75px 95px;
  box-sizing: border-box;
  box-shadow: 0px 4px 4px rgba(0, 0, 0, 0.16);
  border-radius: 4px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  h1 {
    display: flex;
    justify-content: center;
    align-items: center;
    flex: 3 1 auto;
  }
  form {
    flex: 3 1 auto;
    height: max-content;
    width: max-content;
  }
  button {
    width: 100%;
    height: 36px;
    background-color: ${({ theme }) => theme.colors.btn.secondary};
    color: ${({ theme }) => theme.colors.bg.ternary};
    border: none;
    text-transform: uppercase;
  }
`;

const LogoContainer = styled.div`
  margin-bottom: 40px;
  position: relative;
`;

const Logo = styled.img``;

const EnvContainer = styled.div`
  height: 100%;
  position: absolute;
  top: 0px;
  right: 0px;
  transform: translateX(100%);
  color: ${({ theme }) => theme.colors.btn.secondary};
`;

const Side = styled.div`
  font-family: "Inter medium";
  font-size: 12px;
  line-height: 24px;
  padding: 1px 10px 0;
  box-sizing: border-box;
`;

const Env = styled.div<{ env: string }>`
  position: absolute;
  right: -10px;
  top: 0px;
  background-color: ${props =>
    props.env === "dev" ? "green" : props.env === "qa" ? "yellow" : "red"};
  line-height: 24px;
  padding: 1px 10px 0;
  border-radius: 9px;
  text-transform: uppercase;
  font-family: "Inter medium";
  font-size: 12px;
  color: white;
  border: 1px solid ${({ theme }) => theme.colors.border.secondary};
  box-sizing: border-box;
  transform: translateX(100%);
`;

const Label = styled.label`
  display: flex;
  flex-direction: column;
  margin-bottom: 40px;
  ${({ theme }) => theme.typography.subtitle2}
`;

const InputWrapper = styled.div`
  position: relative;
  display: flex;
  flex-direction: column;
  justify-content: center;
  width: 100%;
  ${({ theme }) => theme.typography.body1}

  i {
    position: absolute;
  }
  .icon-ic-eye {
    font-size: 2rem;
    right: 0.4rem;
    color: #5294c3;
  }
  .icon-ic-user,
  .icon-ic-lock {
    font-size: 1.5rem;
    top: 0.4rem;
    left: 0.4rem;
    color: black;
    opacity: 0.6;
  }
`;

const StyledButton = styled.button``;
